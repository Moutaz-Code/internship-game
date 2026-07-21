using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class BluetoothHeartRateSensorStub : MonoBehaviour, IBiometricSensor
    {
        [SerializeField] private string deviceId = "bluetooth-heart-rate-stub";
        [SerializeField] private bool available;
        [SerializeField, Range(35f, 220f)] private float simulatedHeartRate = 72f;

        public string DeviceId => deviceId;
        public bool IsAvailable => available;

        public void Configure(bool isAvailable, float heartRate = 72f)
        {
            available = isAvailable;
            simulatedHeartRate = Mathf.Clamp(heartRate, 35f, 220f);
        }

        public void SetAvailable(bool value)
        {
            available = value;
        }

        public bool TryRead(out BiometricSample sample)
        {
            if (!available)
            {
                sample = default;
                return false;
            }

            var variation = Mathf.Sin(Time.unscaledTime * 0.7f) * 2f;
            sample = new BiometricSample(simulatedHeartRate + variation, Time.unscaledTime);
            return true;
        }

        public void ResetDevice()
        {
        }
    }
}
