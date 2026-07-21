using System;
using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class ExternalFeedbackDeviceStub : MonoBehaviour, IFeedbackOutput
    {
        [SerializeField] private string deviceId = "external-device-stub";
        [SerializeField] private FeedbackChannel channels;
        [SerializeField] private bool available;
        [SerializeField] private bool simulateFailure;

        public string DeviceId => deviceId;
        public FeedbackChannel Channels => channels;
        public bool IsAvailable => available;
        public FeedbackSignal LastSignal { get; private set; }

        public void Configure(string id, FeedbackChannel supportedChannels, bool isAvailable = false)
        {
            deviceId = id;
            channels = supportedChannels;
            available = isAvailable;
        }

        public void SetAvailable(bool value)
        {
            available = value;
        }

        public void SetFailureSimulation(bool value)
        {
            simulateFailure = value;
        }

        public bool CanHandle(in FeedbackSignal signal)
        {
            return true;
        }

        public void Apply(in FeedbackSignal signal)
        {
            if (simulateFailure)
            {
                throw new InvalidOperationException($"Simulated failure from {deviceId}.");
            }

            LastSignal = signal;
        }

        public void ResetDevice()
        {
            LastSignal = default;
        }
    }
}
