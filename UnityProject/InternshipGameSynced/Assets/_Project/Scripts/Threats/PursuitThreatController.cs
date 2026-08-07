using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InternshipGame.Prototype
{
    public sealed class PursuitThreatController : MonoBehaviour, IRunStateListener, IThreatStimulusSink, IPrototypeStatusProvider
    {
        [SerializeField] private bool threatEnabled = true;
        [SerializeField] private MonoBehaviour playerContextBehaviour;
        [SerializeField] private MonoBehaviour failureSinkBehaviour;
        [SerializeField] private ThreatRoute route;
        [SerializeField] private MonoBehaviour navigatorBehaviour;
        [SerializeField] private PrototypeMapDefinition mapDefinition;
        [SerializeField] private FeedbackHub feedbackHub;
        [SerializeField] private GameObject visualRoot;
        [SerializeField, Min(0f)] private float investigationSeconds = 1.25f;
        [SerializeField, Min(0f)] private float loseSightGraceSeconds = 2f;
        [SerializeField, Min(0.1f)] private float searchSeconds = 10f;
        [SerializeField, Min(1f)] private float automaticActivationSeconds = 240f;
        [SerializeField, Min(0.1f)] private float captureDistance = 0.85f;
        [SerializeField, Min(0f)] private float eyeHeight = 1.15f;
        [SerializeField, Min(1f)] private float viewDistance = 22f;
        [SerializeField, Range(10f, 180f)] private float fieldOfView = 120f;
        [SerializeField, Min(0f)] private float nearAwarenessDistance = 2.5f;
        [SerializeField, Range(10f, 180f)] private float nearFieldOfView = 170f;
        [SerializeField, Min(0.02f)] private float perceptionIntervalSeconds = 0.08f;
        [SerializeField] private LayerMask sightBlockingLayers = ~0;
        [SerializeField, Min(0.1f)] private float investigateSpeed = 1.65f;
        [SerializeField, Min(0.1f)] private float huntSpeed = 3.85f;
        [SerializeField, Min(0.1f)] private float searchSpeed = 1.8f;
        [SerializeField, Min(0f)] private float observedLeadSeconds = 0.35f;
        [SerializeField, Min(0f)] private float maximumObservedLeadDistance = 1.4f;
        [SerializeField, Min(0.1f)] private float localSearchProbeInterval = 1.35f;
        [SerializeField, Min(0f)] private float localSearchProbeRadius = 3.25f;
        [SerializeField, Min(1f)] private float proximityCueDistance = 8f;

        [Header("V030 Readable Stealth")]
        [SerializeField, Min(0f)] private float concealedDecayPerSecond = 1f;
        [SerializeField, Min(0f)] private float noticeableQuietGainPerSecond = 0.45f;
        [SerializeField, Min(0f)] private float noticeableMovingGainPerSecond = 0.65f;
        [SerializeField, Min(0f)] private float exposedGainPerSecond = 1.35f;
        [SerializeField, Min(1f)] private float sprintGainMultiplier = 1.35f;
        [SerializeField, Range(0.01f, 1f)] private float visualConfirmationThreshold = 1f;
        [SerializeField, Min(0f)] private float nearlyStillSpeed = 0.12f;
        [SerializeField, Range(0f, 1f)] private float hearingThreshold = ThreatHearingPolicy.DefaultThreshold;

        [Header("PT2 Route Adaptation")]
        [SerializeField] private bool routeAdaptationEnabled = true;
        [SerializeField, Min(0.01f)] private float directEvidenceWeight = 1f;
        [SerializeField, Min(0.01f)] private float inferredEvidenceWeight = 0.5f;
        [SerializeField, Min(0.01f)] private float suspiciousThreshold = 1f;
        [SerializeField, Min(0.02f)] private float adaptedThreshold = 2f;
        [SerializeField, Min(0f)] private float inferredEvidenceCooldownSeconds = 4f;
        [SerializeField, Range(0f, 1f)] private float inferredEvidenceMinimumStrength = 0.3f;
        [SerializeField, Min(0f)] private float confidenceDecayDelaySeconds = 60f;
        [SerializeField, Min(0f)] private float confidenceDecayPerSecond = 0.025f;
        [SerializeField, Min(0f)] private float crossWingReductionMultiplier = 1f;
        [SerializeField, Min(1f)] private float suspiciousPatrolWeight = 1.25f;
        [SerializeField, Min(1f)] private float adaptedPatrolWeight = 1.75f;
        [SerializeField, Min(1f)] private float patrolBiasDurationSeconds = 60f;
        [SerializeField, Min(0f)] private float counterAdaptedDurationSeconds = 12f;
        [SerializeField, Min(1)] private int maximumConsecutiveBiasedSelections = 2;
        [SerializeField, Min(0f)] private float adaptiveSearchRedirectDelaySeconds = 2f;
        [SerializeField, Min(1f)] private float suspiciousSearchDurationMultiplier = 1.15f;
        [SerializeField, Min(1f)] private float adaptedSearchDurationMultiplier = 1.35f;
        [SerializeField, Range(0.1f, 1f)] private float counterAdaptedSearchDurationMultiplier = 0.8f;

        private PursuitThreatStateMachine stateMachine;
        private VisualConfirmationModel visualConfirmationModel;
        private ThreatPressureModel pressureModel;
        private IPlayerContext playerContext;
        private IPlayerMotionSource playerMotion;
        private IRunFailureSink failureSink;
        private IThreatNavigator navigator;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private Vector3 lastKnownPlayerPosition;
        private float nextProximityCueTime;
        private float nextInvestigationCueTime;
        private float activeRunElapsed;
        private bool runActive;
        private OwlBehaviorProfile behaviorProfile = OwlBehaviorProfile.PatrolPressure;
        private int patrolIndex;
        private float nextPerceptionTime;
        private float nextSearchProbeTime;
        private int searchProbeIndex;
        private Vector3 currentSearchTarget;
        private int visibleSampleCount;
        private float lastPerceptionSampleTime;
        private Vector3 currentVisualPosition;
        private Vector3 lastConfirmedVisualPosition;
        private float lastConfirmedVisualTime = float.NegativeInfinity;
        private bool hasLastConfirmedVisual;
        private Vector3 acceptedSoundPosition;
        private float acceptedSoundTime = float.NegativeInfinity;
        private bool hasAcceptedSound;
        private ThreatStimulusKind lastStimulusKind;
        private float lastStimulusEvaluatedStrength;
        private bool lastStimulusAccepted;
        private string lastStimulusDecision = "none this run";
        private ThreatKnowledgeSource knowledgeSource = ThreatKnowledgeSource.PatrolTarget;
        private Vector3 currentKnowledgeTarget;
        private readonly RaycastHit[] sightHits = new RaycastHit[24];

        public PursuitThreatState CurrentState => stateMachine?.State ?? PursuitThreatState.Dormant;
        public bool HasLineOfSight { get; private set; }
        public float PlayerDistance { get; private set; } = float.PositiveInfinity;
        public bool IsThreatEnabled => threatEnabled;
        public bool IsRouteAdaptationEnabled => PressureModel.AdaptationEnabled;
        public ThreatPressureSnapshot Pressure => PressureModel.Snapshot;
        public RouteAdaptationSnapshot RouteAdaptation => PressureModel.Adaptation;
        public OwlBehaviorProfile BehaviorProfile => behaviorProfile;
        public float ViewDistance => viewDistance;
        public float FieldOfView => fieldOfView;
        public float HuntSpeed => huntSpeed;
        public float PerceptionIntervalSeconds => perceptionIntervalSeconds;
        public float HearingThreshold => hearingThreshold;
        public float ConfirmationThreshold => visualConfirmationThreshold;
        public int VisibleSampleCount => visibleSampleCount;
        public VisualConfirmationSnapshot VisualConfirmation => visualConfirmationModel?.Snapshot ?? default;
        public ThreatKnowledgeSource KnowledgeSource => knowledgeSource;
        public Vector3 CurrentKnowledgeTarget => currentKnowledgeTarget;
        public bool LastStimulusAccepted => lastStimulusAccepted;
        public float LastStimulusEvaluatedStrength => lastStimulusEvaluatedStrength;
        public string StatusLabel => BuildStatusLabel();

        private ThreatPressureModel PressureModel => pressureModel ??= CreatePressureModel();
        private VisualConfirmationModel VisualModel => visualConfirmationModel ??= CreateVisualConfirmationModel();

        public void Configure(
            MonoBehaviour contextBehaviour,
            MonoBehaviour runFailureSink,
            ThreatRoute threatRoute,
            FeedbackHub hub,
            GameObject visual,
            bool enabled = true)
        {
            playerContextBehaviour = contextBehaviour;
            failureSinkBehaviour = runFailureSink;
            route = threatRoute;
            feedbackHub = hub;
            visualRoot = visual;
            threatEnabled = enabled;
            ResolveDependencies();
        }

        public void ConfigureNavigation(
            MonoBehaviour navigationBehaviour,
            PrototypeMapDefinition definition)
        {
            navigatorBehaviour = navigationBehaviour;
            SetMapDefinition(definition);
            ResolveDependencies();
        }

        private void Awake()
        {
            ResolveDependencies();
            pressureModel = CreatePressureModel();
            pressureModel.SetAdaptationEnabled(routeAdaptationEnabled);
            visualConfirmationModel = CreateVisualConfirmationModel();
            stateMachine = new PursuitThreatStateMachine(
                investigationSeconds,
                loseSightGraceSeconds,
                searchSeconds,
                captureDistance);
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;
            SetVisible(false);
        }

        private void OnEnable()
        {
            if (mapDefinition != null)
            {
                mapDefinition.ZoneEntered -= OnZoneEntered;
                mapDefinition.ZoneEntered += OnZoneEntered;
            }
        }

        private void OnDisable()
        {
            if (mapDefinition != null)
            {
                mapDefinition.ZoneEntered -= OnZoneEntered;
            }
        }

        private void Update()
        {
            if (Keyboard.current?.f3Key.wasPressedThisFrame == true)
            {
                SetRouteAdaptationEnabled(!PressureModel.AdaptationEnabled);
            }

            if (!threatEnabled || stateMachine == null)
            {
                return;
            }

            if (runActive)
            {
                activeRunElapsed += Time.deltaTime;
                PressureModel.SetThreatPosition(transform.position);
                PressureModel.Tick(Time.deltaTime);
                if (stateMachine.State == PursuitThreatState.Dormant && activeRunElapsed >= automaticActivationSeconds)
                {
                    ActivateThreat();
                }
            }

            if (!IsActiveState(stateMachine.State))
            {
                return;
            }

            var view = playerContext?.ViewTransform;
            if (view == null)
            {
                return;
            }

            var eyePosition = transform.position + Vector3.up * eyeHeight;
            PlayerDistance = Vector3.Distance(eyePosition, view.position);
            if (Time.time >= nextPerceptionTime)
            {
                var rawVisible = CanSeePlayer(view);
                var sampleDelta = lastPerceptionSampleTime > 0f
                    ? Mathf.Max(0f, Time.time - lastPerceptionSampleTime)
                    : Mathf.Max(0.02f, perceptionIntervalSeconds);
                lastPerceptionSampleTime = Time.time;
                nextPerceptionTime = Time.time + Mathf.Max(0.02f, perceptionIntervalSeconds);
                var planarSpeed = Vector3.ProjectOnPlane(playerMotion?.Velocity ?? Vector3.zero, Vector3.up).magnitude;
                var confirmation = VisualModel.Tick(
                    visibleSampleCount,
                    playerMotion?.IsCrouching ?? false,
                    planarSpeed,
                    playerMotion?.IsSprinting ?? false,
                    sampleDelta);
                var pursuitContact = stateMachine.State is PursuitThreatState.Hunting or PursuitThreatState.Searching;
                HasLineOfSight = rawVisible && (confirmation.Confirmed || pursuitContact);
                if (HasLineOfSight)
                {
                    currentVisualPosition = ThreatPerceptionMath.PredictObservedPosition(
                        view.position,
                        playerMotion?.Velocity ?? Vector3.zero,
                        observedLeadSeconds,
                        maximumObservedLeadDistance);
                    lastConfirmedVisualPosition = currentVisualPosition;
                    lastConfirmedVisualTime = Time.time;
                    hasLastConfirmedVisual = true;
                    hasAcceptedSound = false;
                    lastKnownPlayerPosition = currentVisualPosition;
                }
            }

            var previous = stateMachine.State;
            stateMachine.Tick(
                Time.deltaTime,
                HasLineOfSight,
                PlayerDistance,
                GetSearchDurationMultiplier());
            if (previous != stateMachine.State)
            {
                HandleStateChanged(previous, stateMachine.State);
            }

            if (stateMachine.State == PursuitThreatState.Caught)
            {
                feedbackHub?.Send(new FeedbackSignal(
                    PrototypeFeedbackCues.OwlCaught,
                    FeedbackCommand.Pulse,
                    FeedbackChannel.ControllerHaptics | FeedbackChannel.FloorVibration,
                    1f,
                    0.35f));
                failureSink?.TryFailRun("Captured by the Owl Thing.");
                navigator?.Stop();
                return;
            }

            MoveThreat(currentVisualPosition);
            EmitProximityCue();
        }

        public void SetThreatEnabled(bool enabled)
        {
            threatEnabled = enabled;
            if (!enabled)
            {
                ResetThreat();
            }
        }

        public void SetRouteAdaptationEnabled(bool enabled)
        {
            routeAdaptationEnabled = enabled;
            PressureModel.SetAdaptationEnabled(enabled);
        }

        public void OnRunReset()
        {
            runActive = true;
            activeRunElapsed = 0f;
            PressureModel.Reset();
            ResetThreat();
        }

        public void OnRunStateChanged(PrototypeRunState previous, PrototypeRunState current)
        {
            if (!threatEnabled)
            {
                return;
            }

            if (current == PrototypeRunState.RetrieveFuelRods)
            {
                runActive = true;
                ResetThreat();
            }
            else if (current is PrototypeRunState.ExtractionCycling or PrototypeRunState.EscapeReady)
            {
                runActive = true;
                ActivateThreat();
            }
            else if (current is PrototypeRunState.Extracted or PrototypeRunState.Failed)
            {
                runActive = false;
                stateMachine?.Deactivate();
                navigator?.Stop();
                StopFeedback();
                SetVisible(false);
            }
        }

        public void ActivateThreat()
        {
            ActivateThreat(preserveLastKnownPosition: false);
        }

        private void ActivateThreat(bool preserveLastKnownPosition)
        {
            if (!threatEnabled || stateMachine == null || !stateMachine.Activate())
            {
                return;
            }

            if (!preserveLastKnownPosition)
            {
                lastKnownPlayerPosition = spawnPosition;
            }

            nextProximityCueTime = 0f;
            nextInvestigationCueTime = 0f;
            SetVisible(true);
            feedbackHub?.Send(new FeedbackSignal(
                PrototypeFeedbackCues.OwlWarning,
                FeedbackCommand.Pulse,
                FeedbackChannel.Audio,
                0.65f,
                1.15f));
            feedbackHub?.Send(new FeedbackSignal(
                PrototypeFeedbackCues.OwlDetected,
                FeedbackCommand.Set,
                FeedbackChannel.Visual,
                0.35f));
        }

        public void ResetThreat()
        {
            stateMachine?.Reset();
            if (navigator != null)
            {
                navigator.Warp(spawnPosition, spawnRotation);
            }
            else
            {
                transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            }
            lastKnownPlayerPosition = spawnPosition;
            HasLineOfSight = false;
            PlayerDistance = float.PositiveInfinity;
            nextProximityCueTime = 0f;
            nextInvestigationCueTime = 0f;
            nextPerceptionTime = 0f;
            nextSearchProbeTime = 0f;
            searchProbeIndex = 0;
            currentSearchTarget = spawnPosition;
            visibleSampleCount = 0;
            lastPerceptionSampleTime = 0f;
            currentVisualPosition = spawnPosition;
            lastConfirmedVisualPosition = spawnPosition;
            lastConfirmedVisualTime = float.NegativeInfinity;
            hasLastConfirmedVisual = false;
            acceptedSoundPosition = spawnPosition;
            acceptedSoundTime = float.NegativeInfinity;
            hasAcceptedSound = false;
            lastStimulusKind = default;
            lastStimulusEvaluatedStrength = 0f;
            lastStimulusAccepted = false;
            lastStimulusDecision = "none this run";
            knowledgeSource = ThreatKnowledgeSource.PatrolTarget;
            currentKnowledgeTarget = spawnPosition;
            VisualModel.Reset();
            patrolIndex = 0;
            StopFeedback();
            SetVisible(false);
        }

        public void NotifyFirstRodPickup(Vector3 position)
        {
            ReportStimulus(new ThreatStimulus(
                ThreatStimulusKind.ObjectiveProgress,
                position,
                0.55f,
                18f,
                Mathf.Max(0f, Time.time)));
            // First pickup activates the encounter, but an unheard pickup does not
            // grant the Owl the player's exact hidden position.
            if (!lastStimulusAccepted)
            {
                lastKnownPlayerPosition = spawnPosition;
            }

            ActivateThreat(preserveLastKnownPosition: true);
        }

        public void SetObjectiveProgress(float normalizedProgress)
        {
            PressureModel.SetObjectiveProgress(normalizedProgress);
        }

        public void ApplyBehaviorProfile(OwlBehaviorProfile profile)
        {
            behaviorProfile = profile;
        }

        public void SetRouteProfile(string routeProfileId)
        {
            (navigator as IThreatRouteProfileReceiver)?.SetRouteProfile(routeProfileId);
        }

        public void ReportStimulus(ThreatStimulus stimulus)
        {
            var model = PressureModel;
            model.SetThreatPosition(transform.position);
            model.ReportStimulus(stimulus);
            var hearing = ThreatHearingPolicy.Evaluate(
                stimulus,
                transform.position,
                hearingThreshold);
            lastStimulusKind = stimulus.Kind;
            lastStimulusEvaluatedStrength = hearing.EvaluatedStrength;
            lastStimulusAccepted = hearing.Accepted;
            lastStimulusDecision = hearing.Accepted
                ? $"accepted at {hearing.EvaluatedStrength:0.00} (threshold {hearing.Threshold:0.00})"
                : $"rejected: {hearing.RejectionReason} ({hearing.EvaluatedStrength:0.00} < {hearing.Threshold:0.00})";

            var previousAdaptationState = model.Adaptation.State;
            var routeEvidenceRecorded = ThreatRouteEvidencePolicy.TryRecordInferredStimulus(
                model,
                mapDefinition != null
                    ? mapDefinition.GetRouteFamilyAt(stimulus.Position).ToThreatWing()
                    : ThreatWing.None,
                runActive,
                stimulus,
                model.LastStimulusStrength,
                inferredEvidenceMinimumStrength);
            if (routeEvidenceRecorded)
            {
                HandleRouteAdaptationChanged(previousAdaptationState, model.Adaptation.State);
            }

            if (!hearing.Accepted)
            {
                return;
            }

            acceptedSoundPosition = stimulus.Position;
            acceptedSoundTime = Time.time;
            hasAcceptedSound = true;
            if (!HasLineOfSight)
            {
                lastKnownPlayerPosition = acceptedSoundPosition;
                EmitInvestigationCue();
            }

            var previousState = stateMachine?.State ?? PursuitThreatState.Dormant;
            stateMachine?.Investigate();
            if (stateMachine != null && previousState != stateMachine.State)
            {
                HandleStateChanged(previousState, stateMachine.State);
            }
            if (runActive && stateMachine?.State == PursuitThreatState.Dormant)
            {
                ActivateThreat(preserveLastKnownPosition: true);
            }
        }

        private void MoveThreat(Vector3 currentPlayerPosition)
        {
            if (stateMachine.State is PursuitThreatState.Investigating or PursuitThreatState.Searching)
            {
                if (hasAcceptedSound && HasReachedKnowledgePosition(acceptedSoundPosition))
                {
                    hasAcceptedSound = false;
                }
                else if (!hasAcceptedSound && hasLastConfirmedVisual &&
                         HasReachedKnowledgePosition(lastConfirmedVisualPosition))
                {
                    hasLastConfirmedVisual = false;
                }
            }

            var patrolTarget = stateMachine.State == PursuitThreatState.Patrolling
                ? GetPatrolTarget()
                : spawnPosition;
            var searchReference = hasAcceptedSound
                ? acceptedSoundPosition
                : hasLastConfirmedVisual
                    ? lastConfirmedVisualPosition
                    : lastKnownPlayerPosition;
            var hasLocalSearchProbe = false;
            var localSearchTarget = searchReference;
            if (stateMachine.State == PursuitThreatState.Searching)
            {
                searchReference = GetAdaptiveSearchReference(searchReference);
                localSearchTarget = GetLocalSearchTarget(searchReference);
                hasLocalSearchProbe = true;
            }

            var decision = ThreatKnowledgePolicy.Select(
                HasLineOfSight,
                currentPlayerPosition,
                hasAcceptedSound,
                acceptedSoundPosition,
                hasLastConfirmedVisual,
                lastConfirmedVisualPosition,
                hasLocalSearchProbe,
                localSearchTarget,
                patrolTarget);
            knowledgeSource = decision.Source;
            currentKnowledgeTarget = decision.Position;
            lastKnownPlayerPosition = decision.Position;
            var pursuitTarget = stateMachine.State == PursuitThreatState.Patrolling
                ? patrolTarget
                : decision.Position;
            var targetPoint = route != null
                ? route.GetNextPointToward(transform.position, pursuitTarget)
                : pursuitTarget;
            targetPoint.y = spawnPosition.y;

            var speed = stateMachine.State switch
            {
                PursuitThreatState.Hunting => huntSpeed,
                PursuitThreatState.Searching => searchSpeed,
                _ => investigateSpeed,
            };
            speed *= behaviorProfile switch
            {
                OwlBehaviorProfile.PatientSearch => 0.9f,
                OwlBehaviorProfile.NoiseSensitive => 1.06f,
                _ => 1f,
            };

            if (navigator != null)
            {
                navigator.SetDestination(targetPoint, speed);
                return;
            }

            var previousPosition = transform.position;
            transform.position = Vector3.MoveTowards(previousPosition, targetPoint, speed * Time.deltaTime);
            var movement = transform.position - previousPosition;
            movement.y = 0f;
            if (movement.sqrMagnitude > 0.0001f)
            {
                var targetRotation = Quaternion.LookRotation(movement.normalized, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
            }
            else if (stateMachine.State == PursuitThreatState.Searching)
            {
                transform.Rotate(0f, 35f * Time.deltaTime, 0f, Space.World);
            }
        }

        private bool HasReachedKnowledgePosition(Vector3 position)
        {
            var offset = Vector3.ProjectOnPlane(position - transform.position, Vector3.up);
            return offset.sqrMagnitude <= 0.49f;
        }

        internal bool CanSeePlayer(Transform view)
        {
            if (view == null)
            {
                return false;
            }

            visibleSampleCount = 0;
            var root = playerContext?.PlayerRoot;
            var visible = CanSeePoint(view.position);
            visibleSampleCount += visible ? 1 : 0;
            if (root == null)
            {
                return visible;
            }

            var upperBodyVisible = CanSeePoint(root.position + Vector3.up * 1.1f);
            var lowerBodyVisible = CanSeePoint(root.position + Vector3.up * 0.62f);
            visibleSampleCount += upperBodyVisible ? 1 : 0;
            visibleSampleCount += lowerBodyVisible ? 1 : 0;
            return visible || upperBodyVisible || lowerBodyVisible;
        }

        private bool CanSeePoint(Vector3 targetPosition)
        {
            var eyePosition = transform.position + Vector3.up * eyeHeight;
            if (!ThreatPerceptionMath.IsWithinViewCone(
                    eyePosition,
                    transform.forward,
                    targetPosition,
                    viewDistance,
                    fieldOfView,
                    nearAwarenessDistance,
                    nearFieldOfView))
            {
                return false;
            }

            var toTarget = targetPosition - eyePosition;
            var distance = toTarget.magnitude;
            var hitCount = Physics.RaycastNonAlloc(
                eyePosition,
                toTarget / distance,
                sightHits,
                distance + 0.05f,
                sightBlockingLayers,
                QueryTriggerInteraction.Ignore);
            var playerRoot = playerContext?.PlayerRoot;
            var nearestDistance = float.PositiveInfinity;
            Transform nearestTransform = null;
            for (var i = 0; i < hitCount; i++)
            {
                var hit = sightHits[i];
                if (hit.collider == null ||
                    (hit.transform == transform || hit.transform.IsChildOf(transform)) ||
                    hit.distance >= nearestDistance)
                {
                    continue;
                }

                nearestDistance = hit.distance;
                nearestTransform = hit.transform;
            }

            return nearestTransform == null ||
                   playerRoot != null &&
                   (nearestTransform == playerRoot || nearestTransform.IsChildOf(playerRoot));
        }

        private void HandleStateChanged(PursuitThreatState previous, PursuitThreatState current)
        {
            if (current == PursuitThreatState.Searching)
            {
                currentSearchTarget = lastKnownPlayerPosition;
                nextSearchProbeTime = Time.time + 0.75f;
                searchProbeIndex = 0;
            }
            else if (current == PursuitThreatState.Patrolling)
            {
                hasAcceptedSound = false;
                hasLastConfirmedVisual = false;
                knowledgeSource = ThreatKnowledgeSource.PatrolTarget;
            }

            if (current == PursuitThreatState.Hunting)
            {
                feedbackHub?.Send(new FeedbackSignal(
                    PrototypeFeedbackCues.OwlWarning,
                    FeedbackCommand.Pulse,
                    FeedbackChannel.Audio,
                    1f,
                    1.15f));
                feedbackHub?.Send(new FeedbackSignal(
                    PrototypeFeedbackCues.OwlDetected,
                    FeedbackCommand.Set,
                    FeedbackChannel.Visual,
                    1f));
            }
            else if (current is PursuitThreatState.Searching or PursuitThreatState.Investigating)
            {
                feedbackHub?.Send(new FeedbackSignal(
                    PrototypeFeedbackCues.OwlDetected,
                    FeedbackCommand.Set,
                    FeedbackChannel.Visual,
                    0.25f));
            }
            else if (current == PursuitThreatState.Patrolling)
            {
                feedbackHub?.Send(new FeedbackSignal(
                    PrototypeFeedbackCues.OwlDetected,
                    FeedbackCommand.Stop,
                    FeedbackChannel.Visual));
            }
        }

        private void EmitProximityCue()
        {
            if (feedbackHub == null || PlayerDistance > proximityCueDistance || Time.time < nextProximityCueTime)
            {
                return;
            }

            var intensity = Mathf.InverseLerp(proximityCueDistance, captureDistance, PlayerDistance);
            feedbackHub.Send(new FeedbackSignal(
                PrototypeFeedbackCues.OwlProximity,
                FeedbackCommand.Pulse,
                FeedbackChannel.ControllerHaptics | FeedbackChannel.FloorVibration,
                Mathf.Lerp(0.18f, 0.75f, intensity),
                0.18f));
            nextProximityCueTime = Time.time + Mathf.Lerp(1.25f, 0.35f, intensity);
        }

        private void EmitInvestigationCue()
        {
            if (feedbackHub == null || Time.time < nextInvestigationCueTime)
            {
                return;
            }

            feedbackHub.Send(new FeedbackSignal(
                PrototypeFeedbackCues.OwlInvestigation,
                FeedbackCommand.Pulse,
                FeedbackChannel.Audio,
                0.42f,
                0.42f));
            nextInvestigationCueTime = Time.time + 1.25f;
        }

        private void StopFeedback()
        {
            feedbackHub?.Send(new FeedbackSignal(
                PrototypeFeedbackCues.OwlDetected,
                FeedbackCommand.Stop,
                FeedbackChannel.Visual));
            feedbackHub?.Send(new FeedbackSignal(
                PrototypeFeedbackCues.OwlWarning,
                FeedbackCommand.Stop,
                FeedbackChannel.Audio));
            feedbackHub?.Send(new FeedbackSignal(
                PrototypeFeedbackCues.OwlInvestigation,
                FeedbackCommand.Stop,
                FeedbackChannel.Audio));
            feedbackHub?.Send(new FeedbackSignal(
                PrototypeFeedbackCues.OwlProximity,
                FeedbackCommand.Stop,
                FeedbackChannel.ControllerHaptics | FeedbackChannel.FloorVibration));
        }

        private void SetVisible(bool visible)
        {
            if (visualRoot != null && visualRoot.activeSelf != visible)
            {
                visualRoot.SetActive(visible);
            }
        }

        private void ResolveDependencies()
        {
            playerContext = playerContextBehaviour as IPlayerContext;
            playerMotion = playerContext?.PlayerRoot != null
                ? playerContext.PlayerRoot.GetComponent<IPlayerMotionSource>()
                : null;
            failureSink = failureSinkBehaviour as IRunFailureSink;
            navigator = navigatorBehaviour as IThreatNavigator;
        }

        private ThreatPressureModel CreatePressureModel()
        {
            var safeSuspiciousThreshold = Mathf.Max(0.01f, suspiciousThreshold);
            var tuning = new RouteAdaptationTuning(
                Mathf.Max(0.01f, directEvidenceWeight),
                Mathf.Max(0.01f, inferredEvidenceWeight),
                safeSuspiciousThreshold,
                Mathf.Max(safeSuspiciousThreshold + 0.01f, adaptedThreshold),
                Mathf.Max(0f, inferredEvidenceCooldownSeconds),
                Mathf.Max(0f, confidenceDecayDelaySeconds),
                Mathf.Max(0f, confidenceDecayPerSecond),
                Mathf.Max(0f, crossWingReductionMultiplier),
                Mathf.Max(1f, suspiciousPatrolWeight),
                Mathf.Max(1f, adaptedPatrolWeight),
                Mathf.Max(1f, patrolBiasDurationSeconds),
                Mathf.Max(0f, counterAdaptedDurationSeconds),
                Mathf.Max(1, maximumConsecutiveBiasedSelections));
            return new ThreatPressureModel(tuning);
        }

        private VisualConfirmationModel CreateVisualConfirmationModel()
        {
            return new VisualConfirmationModel(new VisualConfirmationTuning(
                Mathf.Max(0f, concealedDecayPerSecond),
                Mathf.Max(0f, noticeableQuietGainPerSecond),
                Mathf.Max(0f, noticeableMovingGainPerSecond),
                Mathf.Max(0f, exposedGainPerSecond),
                Mathf.Max(1f, sprintGainMultiplier),
                Mathf.Clamp(visualConfirmationThreshold, 0.01f, 1f),
                Mathf.Max(0f, nearlyStillSpeed)));
        }

        private float GetSearchDurationMultiplier()
        {
            if (!PressureModel.AdaptationEnabled)
            {
                return 1f;
            }

            return PressureModel.Adaptation.State switch
            {
                RouteAdaptationState.Suspicious => Mathf.Max(1f, suspiciousSearchDurationMultiplier),
                RouteAdaptationState.Adapted => Mathf.Max(1f, adaptedSearchDurationMultiplier),
                RouteAdaptationState.CounterAdapted => Mathf.Clamp(
                    counterAdaptedSearchDurationMultiplier,
                    0.1f,
                    1f),
                _ => 1f,
            };
        }

        private void HandleRouteAdaptationChanged(
            RouteAdaptationState previous,
            RouteAdaptationState current)
        {
            if (previous == current || feedbackHub == null || !IsActiveState(CurrentState))
            {
                return;
            }

            var intensity = current switch
            {
                RouteAdaptationState.Suspicious => 0.28f,
                RouteAdaptationState.Adapted => 0.42f,
                RouteAdaptationState.CounterAdapted => 0.2f,
                _ => 0f,
            };
            if (intensity <= 0f)
            {
                return;
            }

            feedbackHub.Send(new FeedbackSignal(
                PrototypeFeedbackCues.OwlWarning,
                FeedbackCommand.Pulse,
                FeedbackChannel.Audio,
                intensity,
                0.65f));
        }

        private string BuildStatusLabel()
        {
            if (!threatEnabled)
            {
                return "Owl: disabled";
            }

            var adaptation = PressureModel.Adaptation;
            var evidence = adaptation.LastEvidence;
            var evidenceLine = evidence.HasValue
                ? evidence.Value.HasObservedPosition
                    ? $"Evidence: {evidence.Value.Source}/{evidence.Value.Kind} | {evidence.Value.ZoneLabel} " +
                      $"({evidence.Value.ObservedPosition.x:0.0},{evidence.Value.ObservedPosition.z:0.0}) | t {evidence.Value.OccurredAtSeconds:0.0}s"
                    : $"Evidence: {evidence.Value.Source}/{evidence.Value.Kind} | {evidence.Value.ZoneLabel} | t {evidence.Value.OccurredAtSeconds:0.0}s"
                : "Evidence: none this run";
            var counterTiming = adaptation.State == RouteAdaptationState.CounterAdapted
                ? $" | opening {adaptation.CounterAdaptedSecondsRemaining:0.0}s"
                : string.Empty;
            var tuning = PressureModel.RouteTuning;
            var visual = VisualModel.Snapshot;
            var visualAge = float.IsNegativeInfinity(lastConfirmedVisualTime)
                ? "never"
                : $"{Mathf.Max(0f, Time.time - lastConfirmedVisualTime):0.0}s";
            var soundAge = float.IsNegativeInfinity(acceptedSoundTime)
                ? "never"
                : $"{Mathf.Max(0f, Time.time - acceptedSoundTime):0.0}s";
            var stimulusLine = lastStimulusDecision == "none this run"
                ? "Stimulus: none this run"
                : $"Stimulus: {lastStimulusKind} {lastStimulusDecision}";
            var navMeshNavigator = navigatorBehaviour as NavMeshThreatNavigator;
            var navigationLine = navMeshNavigator != null
                ? $"Navigation: node {(mapDefinition != null ? mapDefinition.GetNearestPatrolNodeId(transform.position) : "none")} | destination ({currentKnowledgeTarget.x:0.0},{currentKnowledgeTarget.z:0.0}) | mode {navMeshNavigator.ActiveMode} | primary rejection {navMeshNavigator.LastPrimaryRejectionReason} | graph fallback {(navMeshNavigator.ActiveMode == ThreatNavigationMode.WaypointFallback ? "ACTIVE" : navMeshNavigator.HasWaypointFallback ? "ready" : "unavailable")}"
                : $"Navigation: node none | destination ({currentKnowledgeTarget.x:0.0},{currentKnowledgeTarget.z:0.0}) | mode contract | graph fallback unknown";

            return
                $"Owl: {CurrentState} | LOS {(HasLineOfSight ? "YES" : "no")} | {PlayerDistance:0.0}m | pressure {Pressure.TotalPressure:0.00}\n" +
                $"Vision: {visual.Exposure} {visibleSampleCount}/3 | confirm {visual.Confirmation:0.00}/{visualConfirmationThreshold:0.00} @ {visual.RatePerSecond:+0.00;-0.00;0.00}/s | contact {(HasLineOfSight ? "actionable" : "unconfirmed/none")}\n" +
                $"Perception: {viewDistance:0}m/{fieldOfView:0}deg | sample {perceptionIntervalSeconds * 1000f:0}ms | threshold hear {hearingThreshold:0.00} route {inferredEvidenceMinimumStrength:0.00}\n" +
                stimulusLine + "\n" +
                $"Knowledge: {knowledgeSource} -> ({currentKnowledgeTarget.x:0.0},{currentKnowledgeTarget.z:0.0}) | sight outranks sound\n" +
                $"Memory: visual {visualAge} ({lastConfirmedVisualPosition.x:0.0},{lastConfirmedVisualPosition.z:0.0}) | sound {soundAge} ({acceptedSoundPosition.x:0.0},{acceptedSoundPosition.z:0.0})\n" +
                $"Speeds: investigate {investigateSpeed:0.0} hunt {huntSpeed:0.0} search {searchSpeed:0.0}\n" +
                $"Spatial: {(mapDefinition != null ? mapDefinition.CurrentSpatialLabel : "unknown")} | connector {(mapDefinition != null ? mapDefinition.CurrentRouteFamily : FacilityRouteFamily.None)}\n" +
                $"Route: {(mapDefinition != null ? mapDefinition.ActiveRouteProfileId : "unknown")} | {navigationLine}\n" +
                $"Adapt: {(adaptation.Enabled ? "ON" : "OFF")} {adaptation.State} | North {adaptation.WestConfidence:0.00} Utility {adaptation.CentralConfidence:0.00} South {adaptation.EastConfidence:0.00} | dom {GetRouteFamilyLabel(adaptation.DominantWing)}\n" +
                evidenceLine + "\n" +
                $"Timing: bias {adaptation.BiasSecondsRemaining:0.0}s | decay in {adaptation.SecondsUntilDecay:0.0}s{counterTiming}\n" +
                $"Response: {adaptation.SelectedResponse} | transition {adaptation.LastTransitionFrom}->{adaptation.LastTransitionTo} @ {adaptation.LastTransitionAtSeconds:0.0}s\n" +
                $"Tune: D {tuning.DirectEvidenceWeight:0.##} I {tuning.InferredEvidenceWeight:0.##} | S {tuning.SuspiciousThreshold:0.##} A {tuning.AdaptedThreshold:0.##} | CD {tuning.InferredEvidenceCooldownSeconds:0.#}s decay {tuning.ConfidenceDecayPerSecond:0.###}/s\n" +
                $"Reset: #{adaptation.ResetSequence} {adaptation.LastResetReason} | F3 adaptation A/B";
        }

        private void OnZoneEntered(FacilityZoneId zone)
        {
            var wing = zone.ToThreatWing();
            var view = playerContext?.ViewTransform;
            var threatIsActive = stateMachine != null && IsActiveState(stateMachine.State);
            var hasLineOfSight = view != null && threatIsActive && HasLineOfSight;
            var previousAdaptationState = PressureModel.Adaptation.State;
            var routeEvidenceRecorded = ThreatRouteEvidencePolicy.TryRecordDirectObservation(
                PressureModel,
                wing,
                threatIsActive,
                hasLineOfSight,
                zone.ToString(),
                view != null ? view.position : default);
            if (!routeEvidenceRecorded)
            {
                return;
            }

            lastKnownPlayerPosition = view.position;
            HandleRouteAdaptationChanged(previousAdaptationState, PressureModel.Adaptation.State);
        }

        private static bool IsActiveState(PursuitThreatState state)
        {
            return state is PursuitThreatState.Patrolling or PursuitThreatState.Investigating or PursuitThreatState.Hunting or PursuitThreatState.Searching;
        }

        private Vector3 GetPatrolTarget()
        {
            var points = mapDefinition?.OwlPatrolPoints;
            if (points == null || points.Count == 0)
            {
                return spawnPosition;
            }

            var current = points[Mathf.Clamp(patrolIndex, 0, points.Count - 1)];
            if (current == null || Vector3.Distance(transform.position, current.position) <= 0.5f)
            {
                var patrolWings = new ThreatWing[points.Count];
                for (var i = 0; i < points.Count; i++)
                {
                    patrolWings[i] = mapDefinition.GetPatrolThreatWing(i);
                }

                var model = PressureModel;
                var preferredWing = model.PreferredPatrolWing;
                if (model.GetPatrolWeight(preferredWing) <= 1f)
                {
                    preferredWing = ThreatWing.None;
                }

                var selection = ThreatPatrolSelectionPolicy.SelectNext(
                    patrolWings,
                    patrolIndex,
                    preferredWing,
                    model.RequiresPatrolVariety);
                if (selection.Index >= 0)
                {
                    patrolIndex = selection.Index;
                    current = points[patrolIndex];
                    var response = selection.ForcedVariety
                        ? RouteAdaptationResponse.ForcedPatrolVariety
                        : selection.UsedPreference
                            ? model.Adaptation.State == RouteAdaptationState.Adapted
                                ? RouteAdaptationResponse.AdaptedRouteBias
                                : RouteAdaptationResponse.SuspiciousRouteInspection
                            : model.Adaptation.State == RouteAdaptationState.CounterAdapted
                                ? RouteAdaptationResponse.CounterAdaptationOpening
                                : RouteAdaptationResponse.BaselinePatrol;
                    model.RecordPatrolSelection(selection.Wing, response);
                }
            }

            return current != null ? current.position : spawnPosition;
        }

        private Vector3 GetAdaptiveSearchReference(Vector3 fallback)
        {
            var model = PressureModel;
            var adaptation = model.Adaptation;
            if (!adaptation.Enabled)
            {
                model.SetSelectedResponse(RouteAdaptationResponse.BaselinePatrol);
                return fallback;
            }

            if (adaptation.State == RouteAdaptationState.CounterAdapted)
            {
                model.SetSelectedResponse(RouteAdaptationResponse.CounterAdaptationOpening);
                return fallback;
            }

            var preferredWing = model.PreferredPatrolWing;
            if (preferredWing == ThreatWing.None ||
                stateMachine.StateElapsed < adaptiveSearchRedirectDelaySeconds)
            {
                return fallback;
            }

            var points = mapDefinition?.OwlPatrolPoints;
            Transform bestPoint = null;
            var bestDistance = float.PositiveInfinity;
            if (points != null)
            {
                for (var pointIndex = 0; pointIndex < points.Count; pointIndex++)
                {
                    var point = points[pointIndex];
                    if (point == null || mapDefinition.GetPatrolThreatWing(pointIndex) != preferredWing)
                    {
                        continue;
                    }

                    var distance = Vector3.SqrMagnitude(point.position - fallback);
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bestPoint = point;
                    }
                }
            }

            if (bestPoint == null)
            {
                return fallback;
            }

            model.SetSelectedResponse(
                adaptation.State == RouteAdaptationState.Adapted
                    ? RouteAdaptationResponse.AdaptedRouteSearch
                    : RouteAdaptationResponse.SuspiciousRouteSearch);
            return bestPoint.position;
        }

        private Vector3 GetLocalSearchTarget(Vector3 reference)
        {
            if (Time.time < nextSearchProbeTime)
            {
                return currentSearchTarget;
            }

            currentSearchTarget = reference + ThreatPerceptionMath.SearchOffset(
                searchProbeIndex++,
                localSearchProbeRadius);
            currentSearchTarget.y = reference.y;
            nextSearchProbeTime = Time.time + Mathf.Max(0.1f, localSearchProbeInterval);
            return currentSearchTarget;
        }

        private void SetMapDefinition(PrototypeMapDefinition definition)
        {
            if (mapDefinition == definition)
            {
                return;
            }

            if (isActiveAndEnabled && mapDefinition != null)
            {
                mapDefinition.ZoneEntered -= OnZoneEntered;
            }

            mapDefinition = definition;
            if (isActiveAndEnabled && mapDefinition != null)
            {
                mapDefinition.ZoneEntered -= OnZoneEntered;
                mapDefinition.ZoneEntered += OnZoneEntered;
            }
        }

        private static string GetRouteFamilyLabel(ThreatWing wing)
        {
            return wing switch
            {
                ThreatWing.West => "North Pipe Gallery",
                ThreatWing.Central => "Utility Spine",
                ThreatWing.East => "South Maintenance Channel",
                _ => "None",
            };
        }
    }
}
