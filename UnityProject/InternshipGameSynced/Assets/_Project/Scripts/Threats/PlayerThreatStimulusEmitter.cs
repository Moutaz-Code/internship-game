using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class PlayerThreatStimulusEmitter : MonoBehaviour, IRunStateListener, IPrototypeStatusProvider
    {
        [SerializeField] private MonoBehaviour playerContextBehaviour;
        [SerializeField] private MonoBehaviour motionSourceBehaviour;
        [SerializeField] private MonoBehaviour stimulusSinkBehaviour;
        [SerializeField] private ProceduralSpatialAudio footstepAudio;

        private IPlayerContext playerContext;
        private IPlayerMotionSource motionSource;
        private IThreatStimulusSink stimulusSink;
        private readonly MovementNoisePulseScheduler movementScheduler = new();
        private bool emissionEnabled = true;
        private float lastPulseTime = float.NegativeInfinity;
        private MovementNoiseProfile lastPulseProfile;

        public MovementNoiseBand CurrentMovementBand { get; private set; }
        public float LastMovementPulseTime => lastPulseTime;
        public string StatusLabel
        {
            get
            {
                var posture = motionSource == null
                    ? "motion unavailable"
                    : $"crouch {(motionSource.IsCrouching ? "yes" : "no")} | sprint {(motionSource.IsSprinting ? "yes" : "no")}";
                var pulse = float.IsNegativeInfinity(lastPulseTime)
                    ? "none this run"
                    : $"{lastPulseProfile.Intensity:0.00}/{lastPulseProfile.Radius:0.#}m @ {lastPulseTime:0.0}s";
                return $"Motion noise: {CurrentMovementBand} | {posture} | last pulse {pulse}";
            }
        }

        public void Configure(
            MonoBehaviour context,
            MonoBehaviour motion,
            MonoBehaviour sink,
            ProceduralSpatialAudio audio = null)
        {
            playerContextBehaviour = context;
            motionSourceBehaviour = motion;
            stimulusSinkBehaviour = sink;
            footstepAudio = audio;
            Resolve();
        }

        private void Awake()
        {
            Resolve();
        }

        private void Update()
        {
            if (!emissionEnabled || motionSource == null || stimulusSink == null)
            {
                return;
            }

            var position = playerContext?.PlayerRoot != null
                ? playerContext.PlayerRoot.position
                : transform.position;
            if (motionSource.LandedThisFrame)
            {
                var landingIntensity = Mathf.InverseLerp(2f, 8f, motionSource.LandingImpactSpeed);
                stimulusSink.ReportStimulus(new ThreatStimulus(
                    ThreatStimulusKind.JumpLanding,
                    position,
                    Mathf.Lerp(0.35f, 0.75f, landingIntensity),
                    15f,
                    Mathf.Max(0f, Time.time)));
            }

            var planarSpeed = Vector3.ProjectOnPlane(motionSource.Velocity, Vector3.up).magnitude;
            CurrentMovementBand = PlayerMovementNoisePolicy.Classify(
                planarSpeed,
                motionSource.IsCrouching,
                motionSource.IsSprinting);
            if (movementScheduler.TryCreatePulse(CurrentMovementBand, Time.time, out var profile))
            {
                var kind = CurrentMovementBand == MovementNoiseBand.Sprint
                    ? ThreatStimulusKind.Sprint
                    : ThreatStimulusKind.Movement;
                stimulusSink.ReportStimulus(new ThreatStimulus(
                    kind,
                    position,
                    profile.Intensity,
                    profile.Radius,
                    Mathf.Max(0f, Time.time)));
                lastPulseTime = Time.time;
                lastPulseProfile = profile;
                footstepAudio?.PlayFootstep(CurrentMovementBand);
            }
        }

        public void OnRunReset()
        {
            movementScheduler.Reset();
            CurrentMovementBand = MovementNoiseBand.Still;
            lastPulseTime = float.NegativeInfinity;
            lastPulseProfile = default;
            emissionEnabled = true;
            footstepAudio?.Stop();
        }

        public void OnRunStateChanged(PrototypeRunState previous, PrototypeRunState current)
        {
            emissionEnabled = current is not PrototypeRunState.Extracted and not PrototypeRunState.Failed;
            if (!emissionEnabled)
            {
                CurrentMovementBand = MovementNoiseBand.Still;
                movementScheduler.Reset();
                footstepAudio?.Stop();
            }
        }

        private void OnDisable()
        {
            CurrentMovementBand = MovementNoiseBand.Still;
            movementScheduler.Reset();
            footstepAudio?.Stop();
        }

        private void Resolve()
        {
            playerContext = playerContextBehaviour as IPlayerContext;
            motionSource = motionSourceBehaviour as IPlayerMotionSource;
            stimulusSink = stimulusSinkBehaviour as IThreatStimulusSink;
        }
    }
}
