using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class ThreatCueController : MonoBehaviour, IRunStateListener, IPrototypeStatusProvider
    {
        [SerializeField] private FeedbackHub feedbackHub;
        [SerializeField, Min(1f)] private float escalationSeconds = 45f;

        private float elapsed;
        private float nextEnvironmentalUpdate;
        private bool cueActive;

        public string StatusLabel => cueActive ? "Threat cue: escalating" : "Threat cue: idle";

        public void Configure(FeedbackHub hub)
        {
            feedbackHub = hub;
        }

        private void Update()
        {
            if (!cueActive || feedbackHub == null)
            {
                return;
            }

            elapsed += Time.deltaTime;
            var escalation = Mathf.Clamp01(elapsed / escalationSeconds);
            var pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * Mathf.Lerp(3f, 8f, escalation));
            var visualIntensity = Mathf.Lerp(0.2f, 1f, escalation) * Mathf.Lerp(0.35f, 1f, pulse);
            feedbackHub.Send(new FeedbackSignal(
                PrototypeFeedbackCues.ThreatAmbient,
                FeedbackCommand.Set,
                FeedbackChannel.Visual,
                visualIntensity));

            if (elapsed >= nextEnvironmentalUpdate)
            {
                feedbackHub.Send(new FeedbackSignal(
                    PrototypeFeedbackCues.ThreatAmbient,
                    FeedbackCommand.Set,
                    FeedbackChannel.Audio | FeedbackChannel.Visual,
                    Mathf.Lerp(0.15f, 0.7f, escalation)));
                nextEnvironmentalUpdate = elapsed + 0.5f;
            }
        }

        public void OnRunReset()
        {
            ResetCue();
        }

        public void OnRunStateChanged(PrototypeRunState previous, PrototypeRunState current)
        {
            if (current == PrototypeRunState.ExtractionCycling)
            {
                StartCue();
            }
            else if (current is PrototypeRunState.RetrieveFuelRods
                or PrototypeRunState.EscapeReady
                or PrototypeRunState.Extracted
                or PrototypeRunState.Failed)
            {
                StopCue();
            }
        }

        public void StartCue()
        {
            cueActive = true;
            elapsed = 0f;
            nextEnvironmentalUpdate = 0f;
            feedbackHub?.Send(new FeedbackSignal(
                PrototypeFeedbackCues.ThreatAmbient,
                FeedbackCommand.Set,
                FeedbackChannel.Audio | FeedbackChannel.Visual,
                0.25f));
        }

        public void StopCue()
        {
            cueActive = false;
            feedbackHub?.Send(new FeedbackSignal(
                PrototypeFeedbackCues.ThreatAmbient,
                FeedbackCommand.Stop,
                FeedbackChannel.Audio | FeedbackChannel.Visual));
        }

        public void ResetCue()
        {
            elapsed = 0f;
            nextEnvironmentalUpdate = 0f;
            StopCue();
        }
    }
}
