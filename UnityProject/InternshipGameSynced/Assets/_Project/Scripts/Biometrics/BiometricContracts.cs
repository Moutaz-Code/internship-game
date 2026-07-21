namespace InternshipGame.Prototype
{
    public readonly struct BiometricSample
    {
        public BiometricSample(float heartRateBpm, float timestamp)
        {
            HeartRateBpm = heartRateBpm;
            Timestamp = timestamp;
        }

        public float HeartRateBpm { get; }
        public float Timestamp { get; }
    }

    public interface IBiometricSensor
    {
        string DeviceId { get; }
        bool IsAvailable { get; }
        bool TryRead(out BiometricSample sample);
        void ResetDevice();
    }
}
