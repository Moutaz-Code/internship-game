using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace InternshipGame.Prototype
{
    public sealed class XRHapticFeedbackOutput : MonoBehaviour, IFeedbackOutput
    {
        [SerializeField] private HapticImpulsePlayer hapticPlayer;

        public string DeviceId => $"xr-haptics:{name}";
        public FeedbackChannel Channels => FeedbackChannel.ControllerHaptics;
        public bool IsAvailable => hapticPlayer != null && hapticPlayer.isActiveAndEnabled;

        public void Configure(HapticImpulsePlayer player)
        {
            hapticPlayer = player;
        }

        public bool CanHandle(in FeedbackSignal signal)
        {
            return signal.Command == FeedbackCommand.Pulse;
        }

        public void Apply(in FeedbackSignal signal)
        {
            if (IsAvailable)
            {
                hapticPlayer.SendHapticImpulse(signal.Intensity, Mathf.Max(0.01f, signal.DurationSeconds));
            }
        }

        public void ResetDevice()
        {
        }
    }
}
