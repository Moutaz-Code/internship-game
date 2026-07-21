using UnityEngine;

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
        [SerializeField, Min(0f)] private float investigationSeconds = 1.75f;
        [SerializeField, Min(0f)] private float loseSightGraceSeconds = 1.5f;
        [SerializeField, Min(0.1f)] private float searchSeconds = 8f;
        [SerializeField, Min(1f)] private float automaticActivationSeconds = 240f;
        [SerializeField, Min(0.1f)] private float captureDistance = 0.85f;
        [SerializeField, Min(0f)] private float eyeHeight = 1.15f;
        [SerializeField, Min(1f)] private float viewDistance = 15f;
        [SerializeField, Range(10f, 180f)] private float fieldOfView = 105f;
        [SerializeField] private LayerMask sightBlockingLayers = ~0;
        [SerializeField, Min(0.1f)] private float investigateSpeed = 1.25f;
        [SerializeField, Min(0.1f)] private float huntSpeed = 2.35f;
        [SerializeField, Min(0.1f)] private float searchSpeed = 1.45f;
        [SerializeField, Min(1f)] private float proximityCueDistance = 8f;

        private PursuitThreatStateMachine stateMachine;
        private readonly ThreatPressureModel pressureModel = new();
        private IPlayerContext playerContext;
        private IRunFailureSink failureSink;
        private IThreatNavigator navigator;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private Vector3 lastKnownPlayerPosition;
        private float nextProximityCueTime;
        private float activeRunElapsed;
        private bool runActive;
        private OwlBehaviorProfile behaviorProfile = OwlBehaviorProfile.PatrolPressure;
        private int patrolIndex;

        public PursuitThreatState CurrentState => stateMachine?.State ?? PursuitThreatState.Dormant;
        public bool HasLineOfSight { get; private set; }
        public float PlayerDistance { get; private set; } = float.PositiveInfinity;
        public bool IsThreatEnabled => threatEnabled;
        public ThreatPressureSnapshot Pressure => pressureModel.Snapshot;
        public OwlBehaviorProfile BehaviorProfile => behaviorProfile;
        public string StatusLabel => threatEnabled
            ? $"Owl: {CurrentState}  LOS: {(HasLineOfSight ? "YES" : "no")}  {PlayerDistance:0.0}m  " +
              $"pressure {Pressure.TotalPressure:0.00}  bias {Pressure.BiasedWing}"
            : "Owl: disabled";

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
            mapDefinition = definition;
            ResolveDependencies();
        }

        private void Awake()
        {
            ResolveDependencies();
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
            if (!threatEnabled || stateMachine == null)
            {
                return;
            }

            if (runActive)
            {
                activeRunElapsed += Time.deltaTime;
                pressureModel.SetThreatPosition(transform.position);
                pressureModel.Tick(Time.deltaTime);
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
            HasLineOfSight = CanSeePlayer(view);
            if (HasLineOfSight)
            {
                lastKnownPlayerPosition = view.position;
            }

            var previous = stateMachine.State;
            stateMachine.Tick(Time.deltaTime, HasLineOfSight, PlayerDistance);
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

            MoveThreat(view.position);
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

        public void OnRunReset()
        {
            runActive = true;
            activeRunElapsed = 0f;
            pressureModel.Reset();
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
                var view = playerContext?.ViewTransform;
                lastKnownPlayerPosition = view != null ? view.position : transform.position;
            }

            nextProximityCueTime = 0f;
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
            lastKnownPlayerPosition = position;
            ActivateThreat(preserveLastKnownPosition: true);
        }

        public void SetObjectiveProgress(float normalizedProgress)
        {
            pressureModel.SetObjectiveProgress(normalizedProgress);
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
            pressureModel.SetThreatPosition(transform.position);
            pressureModel.ReportStimulus(stimulus);
            if (pressureModel.LastStimulusStrength <= 0f)
            {
                return;
            }

            lastKnownPlayerPosition = stimulus.Position;
            if (stateMachine?.State == PursuitThreatState.Patrolling)
            {
                stateMachine.Investigate();
            }

            if (runActive && stateMachine?.State == PursuitThreatState.Dormant)
            {
                ActivateThreat(preserveLastKnownPosition: true);
            }
        }

        private void MoveThreat(Vector3 currentPlayerPosition)
        {
            var patrolTarget = stateMachine.State == PursuitThreatState.Patrolling
                ? GetPatrolTarget()
                : spawnPosition;
            var pursuitTarget = ThreatTargetPolicy.SelectMovementTarget(
                stateMachine.State,
                currentPlayerPosition,
                lastKnownPlayerPosition,
                patrolTarget);
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

        private bool CanSeePlayer(Transform view)
        {
            var eyePosition = transform.position + Vector3.up * eyeHeight;
            var toPlayer = view.position - eyePosition;
            var distance = toPlayer.magnitude;
            if (distance <= 0.001f || distance > viewDistance)
            {
                return false;
            }

            var direction = toPlayer / distance;
            var flatDirection = Vector3.ProjectOnPlane(direction, Vector3.up);
            var flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            if (flatDirection.sqrMagnitude > 0.001f && flatForward.sqrMagnitude > 0.001f &&
                Vector3.Angle(flatForward, flatDirection) > fieldOfView * 0.5f)
            {
                return false;
            }

            if (!Physics.Raycast(
                    eyePosition,
                    direction,
                    out var hit,
                    distance,
                    sightBlockingLayers,
                    QueryTriggerInteraction.Ignore))
            {
                return true;
            }

            var playerRoot = playerContext?.PlayerRoot;
            return playerRoot != null && (hit.transform == playerRoot || hit.transform.IsChildOf(playerRoot));
        }

        private void HandleStateChanged(PursuitThreatState previous, PursuitThreatState current)
        {
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
            failureSink = failureSinkBehaviour as IRunFailureSink;
            navigator = navigatorBehaviour as IThreatNavigator;
        }

        private void OnZoneEntered(FacilityZoneId zone)
        {
            var wing = zone switch
            {
                FacilityZoneId.WestMaintenance or FacilityZoneId.WestRodStorage => ThreatWing.West,
                FacilityZoneId.CentralTransfer or FacilityZoneId.CoreVault => ThreatWing.Central,
                FacilityZoneId.EastLaboratory or FacilityZoneId.EastReactor => ThreatWing.East,
                _ => ThreatWing.None,
            };
            pressureModel.RecordWingVisit(wing);
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
                var desiredWing = pressureModel.Snapshot.BiasedWing;
                for (var offset = 1; offset <= points.Count; offset++)
                {
                    var candidateIndex = (patrolIndex + offset) % points.Count;
                    var candidate = points[candidateIndex];
                    if (candidate == null || (desiredWing != ThreatWing.None && InferWing(candidate.position) != desiredWing))
                    {
                        continue;
                    }

                    patrolIndex = candidateIndex;
                    current = candidate;
                    break;
                }
            }

            return current != null ? current.position : spawnPosition;
        }

        private static ThreatWing InferWing(Vector3 position)
        {
            if (position.x < -3f)
            {
                return ThreatWing.West;
            }

            return position.x > 3f ? ThreatWing.East : ThreatWing.Central;
        }
    }
}
