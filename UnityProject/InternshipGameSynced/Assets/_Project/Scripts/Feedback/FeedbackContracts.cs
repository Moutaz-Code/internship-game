using System;
using UnityEngine;

namespace InternshipGame.Prototype
{
    [Flags]
    public enum FeedbackChannel
    {
        None = 0,
        Visual = 1 << 0,
        Audio = 1 << 1,
        ControllerHaptics = 1 << 2,
        FloorVibration = 1 << 3,
        Heat = 1 << 4,
        Wind = 1 << 5,
    }

    public enum FeedbackCommand
    {
        Set,
        Pulse,
        Stop,
        Reset,
    }

    public readonly struct FeedbackSignal
    {
        public FeedbackSignal(
            string cueId,
            FeedbackCommand command,
            FeedbackChannel channels,
            float intensity = 1f,
            float durationSeconds = 0f)
        {
            CueId = cueId ?? string.Empty;
            Command = command;
            Channels = channels;
            Intensity = Mathf.Clamp01(intensity);
            DurationSeconds = Mathf.Max(0f, durationSeconds);
        }

        public string CueId { get; }
        public FeedbackCommand Command { get; }
        public FeedbackChannel Channels { get; }
        public float Intensity { get; }
        public float DurationSeconds { get; }
    }

    public static class PrototypeFeedbackCues
    {
        public const string ThreatAmbient = "threat.ambient";
        public const string ThreatPulse = "threat.pulse";
        public const string WatcherReveal = "watcher.reveal";
        public const string OwlWarning = "owl.warning";
        public const string OwlDetected = "owl.detected";
        public const string OwlProximity = "owl.proximity";
        public const string OwlCaught = "owl.caught";
        public const string RodAssayStarted = "rod.assay.started";
        public const string RodLiveConfirmed = "rod.live.confirmed";
        public const string RodInertConfirmed = "rod.inert.confirmed";
        public const string RodAssayRequired = "rod.assay.required";
        public const string RodRejected = "rod.rejected";
        public const string RodDeposited = "rod.deposited";
        public const string GeigerClick = "geiger.click";
        public const string ExtractionCycling = "extraction.cycling";
        public const string ExtractionReady = "extraction.ready";
    }

    public interface IFeedbackOutput
    {
        string DeviceId { get; }
        FeedbackChannel Channels { get; }
        bool IsAvailable { get; }
        bool CanHandle(in FeedbackSignal signal);
        void Apply(in FeedbackSignal signal);
        void ResetDevice();
    }

    public sealed class DeviceFaultGate
    {
        private readonly int failureThreshold;
        private readonly float retryDelaySeconds;

        public DeviceFaultGate(int failureThreshold, float retryDelaySeconds)
        {
            this.failureThreshold = Mathf.Max(1, failureThreshold);
            this.retryDelaySeconds = Mathf.Max(0f, retryDelaySeconds);
        }

        public int ConsecutiveFailures { get; private set; }
        public float RetryAt { get; private set; }
        public bool IsFaulted => ConsecutiveFailures >= failureThreshold;

        public bool CanAttempt(float now)
        {
            return !IsFaulted || now >= RetryAt;
        }

        public void MarkSuccess()
        {
            ConsecutiveFailures = 0;
            RetryAt = 0f;
        }

        public void MarkFailure(float now)
        {
            ConsecutiveFailures++;
            if (IsFaulted)
            {
                RetryAt = now + retryDelaySeconds;
            }
        }
    }

    public readonly struct DeviceHealthSnapshot
    {
        public DeviceHealthSnapshot(int configured, int available, int faulted)
        {
            Configured = configured;
            Available = available;
            Faulted = faulted;
        }

        public int Configured { get; }
        public int Available { get; }
        public int Faulted { get; }
    }
}
