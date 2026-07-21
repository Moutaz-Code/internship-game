using UnityEngine;

namespace InternshipGame.Prototype
{
    [RequireComponent(typeof(Light))]
    public sealed class LightFeedbackOutput : MonoBehaviour, IFeedbackOutput
    {
        [SerializeField] private Light lightSource;
        [SerializeField] private string acceptedCueId = PrototypeFeedbackCues.ThreatAmbient;

        private float baseIntensity;

        public string DeviceId => $"light:{name}";
        public FeedbackChannel Channels => FeedbackChannel.Visual;
        public bool IsAvailable => lightSource != null;

        public void Configure(Light source, string cueId)
        {
            lightSource = source;
            acceptedCueId = cueId;
        }

        private void Awake()
        {
            lightSource ??= GetComponent<Light>();
            baseIntensity = lightSource != null ? lightSource.intensity : 1f;
            ResetDevice();
        }

        public bool CanHandle(in FeedbackSignal signal)
        {
            return string.IsNullOrEmpty(acceptedCueId) || signal.CueId == acceptedCueId;
        }

        public void Apply(in FeedbackSignal signal)
        {
            if (lightSource == null)
            {
                return;
            }

            if (signal.Command is FeedbackCommand.Stop or FeedbackCommand.Reset)
            {
                ResetDevice();
                return;
            }

            lightSource.enabled = true;
            lightSource.intensity = baseIntensity * Mathf.Lerp(0.3f, 1.8f, signal.Intensity);
        }

        public void ResetDevice()
        {
            if (lightSource != null)
            {
                lightSource.enabled = false;
                lightSource.intensity = baseIntensity;
            }
        }
    }
}
