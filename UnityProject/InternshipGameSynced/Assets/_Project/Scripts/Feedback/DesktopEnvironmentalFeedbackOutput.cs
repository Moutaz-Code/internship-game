using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class DesktopEnvironmentalFeedbackOutput : MonoBehaviour, IFeedbackOutput, IPrototypeStatusProvider
    {
        private const float DefaultPulseSeconds = 1.1f;

        private float pulseEndsAt;
        private Color pulseColor = Color.clear;
        private string pulseLabel = string.Empty;
        private bool extractionCycling;
        private float extractionIntensity;
        private GUIStyle labelStyle;

        public string DeviceId => "desktop-environment-simulation";
        public FeedbackChannel Channels => FeedbackChannel.Visual | FeedbackChannel.Heat | FeedbackChannel.Wind;
        public bool IsAvailable => true;
        public string StatusLabel => extractionCycling ? "Room FX: simulated wind active" : "Room FX: desktop simulation ready";

        public bool CanHandle(in FeedbackSignal signal)
        {
            return signal.CueId is PrototypeFeedbackCues.RodLiveConfirmed or
                PrototypeFeedbackCues.RodInertConfirmed or
                PrototypeFeedbackCues.RodAssayRequired or
                PrototypeFeedbackCues.RodRejected or
                PrototypeFeedbackCues.RodDeposited or
                PrototypeFeedbackCues.ExtractionCycling or
                PrototypeFeedbackCues.ExtractionReady;
        }

        public void Apply(in FeedbackSignal signal)
        {
            if (signal.Command is FeedbackCommand.Reset)
            {
                ResetDevice();
                return;
            }

            if (signal.CueId == PrototypeFeedbackCues.ExtractionCycling)
            {
                extractionCycling = signal.Command is not FeedbackCommand.Stop;
                extractionIntensity = signal.Intensity;
                return;
            }

            if (signal.Command == FeedbackCommand.Stop)
            {
                pulseEndsAt = 0f;
                return;
            }

            switch (signal.CueId)
            {
                case PrototypeFeedbackCues.RodLiveConfirmed:
                    ShowPulse(new Color(1f, 0.46f, 0.08f, 0.8f), "WARMTH — LIVE ROD CONFIRMED", signal);
                    break;
                case PrototypeFeedbackCues.RodInertConfirmed:
                    ShowPulse(new Color(0.18f, 0.65f, 1f, 0.78f), "COOL RESPONSE — INERT ROD", signal);
                    break;
                case PrototypeFeedbackCues.RodAssayRequired:
                    ShowPulse(new Color(0.82f, 0.86f, 0.9f, 0.65f), "ASSAY REQUIRED — HOLD FOR 3 SECONDS", signal);
                    break;
                case PrototypeFeedbackCues.RodRejected:
                    ShowPulse(new Color(1f, 0.08f, 0.04f, 0.82f), "RACK REJECTED ROD — OWL PRESSURE RISING", signal);
                    break;
                case PrototypeFeedbackCues.RodDeposited:
                    ShowPulse(new Color(0.2f, 1f, 0.48f, 0.72f), "LIVE ROD LOCKED", signal);
                    break;
                case PrototypeFeedbackCues.ExtractionReady:
                    ShowPulse(new Color(0.08f, 0.92f, 1f, 0.85f), "EXIT OPEN — CROSS THE CYAN GATE", signal);
                    break;
            }
        }

        public void ResetDevice()
        {
            pulseEndsAt = 0f;
            pulseColor = Color.clear;
            pulseLabel = string.Empty;
            extractionCycling = false;
            extractionIntensity = 0f;
        }

        private void ShowPulse(Color color, string label, in FeedbackSignal signal)
        {
            pulseColor = color;
            pulseLabel = label;
            pulseEndsAt = Time.unscaledTime + Mathf.Max(DefaultPulseSeconds, signal.DurationSeconds);
        }

        private void OnGUI()
        {
            var pulseActive = Time.unscaledTime < pulseEndsAt;
            if (!pulseActive && !extractionCycling)
            {
                return;
            }

            var color = pulseActive
                ? pulseColor
                : new Color(0.08f, 0.8f, 1f, Mathf.Lerp(0.18f, 0.48f, extractionIntensity));
            if (extractionCycling)
            {
                var throb = 0.65f + 0.35f * Mathf.Sin(Time.unscaledTime * Mathf.PI * 2f);
                color.a *= throb;
            }

            var previousColor = GUI.color;
            GUI.color = color;
            const float edge = 18f;
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, edge), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(0f, Screen.height - edge, Screen.width, edge), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(0f, 0f, edge, Screen.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(Screen.width - edge, 0f, edge, Screen.height), Texture2D.whiteTexture);
            GUI.color = previousColor;

            labelStyle ??= new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white },
            };

            var label = pulseActive ? pulseLabel : "EXTRACTION CYCLING — SIMULATED WIND";
            GUI.Box(new Rect(Screen.width * 0.5f - 260f, 22f, 520f, 38f), GUIContent.none);
            GUI.Label(new Rect(Screen.width * 0.5f - 250f, 24f, 500f, 34f), label, labelStyle);
        }
    }
}
