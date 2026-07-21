using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class PlayerThreatStimulusEmitter : MonoBehaviour, IRunStateListener
    {
        [SerializeField] private MonoBehaviour playerContextBehaviour;
        [SerializeField] private MonoBehaviour motionSourceBehaviour;
        [SerializeField] private MonoBehaviour stimulusSinkBehaviour;
        [SerializeField, Min(0.1f)] private float sprintPulseInterval = 0.65f;

        private IPlayerContext playerContext;
        private IPlayerMotionSource motionSource;
        private IThreatStimulusSink stimulusSink;
        private float nextSprintPulse;
        private bool emissionEnabled = true;

        public void Configure(MonoBehaviour context, MonoBehaviour motion, MonoBehaviour sink)
        {
            playerContextBehaviour = context;
            motionSourceBehaviour = motion;
            stimulusSinkBehaviour = sink;
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

            if (motionSource.IsSprinting && Time.time >= nextSprintPulse)
            {
                stimulusSink.ReportStimulus(new ThreatStimulus(
                    ThreatStimulusKind.Sprint,
                    position,
                    0.42f,
                    11f,
                    Mathf.Max(0f, Time.time)));
                nextSprintPulse = Time.time + sprintPulseInterval;
            }
        }

        public void OnRunReset()
        {
            nextSprintPulse = 0f;
            emissionEnabled = true;
        }

        public void OnRunStateChanged(PrototypeRunState previous, PrototypeRunState current)
        {
            emissionEnabled = current is not PrototypeRunState.Extracted and not PrototypeRunState.Failed;
        }

        private void Resolve()
        {
            playerContext = playerContextBehaviour as IPlayerContext;
            motionSource = motionSourceBehaviour as IPlayerMotionSource;
            stimulusSink = stimulusSinkBehaviour as IThreatStimulusSink;
        }
    }
}
