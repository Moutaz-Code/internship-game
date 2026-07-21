using System;
using System.Collections.Generic;
using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class FeedbackHub : MonoBehaviour, IPrototypeStatusProvider, IRunStateListener
    {
        [SerializeField] private MonoBehaviour[] outputBehaviours = Array.Empty<MonoBehaviour>();
        [SerializeField, Min(1)] private int failureThreshold = 3;
        [SerializeField, Min(0f)] private float retryDelaySeconds = 5f;

        private readonly List<OutputRegistration> outputs = new();

        public string StatusLabel
        {
            get
            {
                var health = GetHealthSnapshot();
                return $"Feedback: {health.Available}/{health.Configured} ready, {health.Faulted} faulted";
            }
        }

        public void Configure(params MonoBehaviour[] configuredOutputs)
        {
            outputBehaviours = configuredOutputs ?? Array.Empty<MonoBehaviour>();
        }

        private void Awake()
        {
            RebuildRegistrations();
        }

        public void Send(in FeedbackSignal signal)
        {
            var signalCopy = signal;
            foreach (var registration in outputs)
            {
                var output = registration.Output;
                if ((output.Channels & signal.Channels) == 0 ||
                    !output.CanHandle(signal) ||
                    !registration.Gate.CanAttempt(Time.unscaledTime))
                {
                    continue;
                }

                if (!output.IsAvailable && !registration.Gate.IsFaulted)
                {
                    continue;
                }

                TryInvoke(registration, () =>
                {
                    if (registration.Gate.IsFaulted)
                    {
                        output.ResetDevice();
                    }

                    if (output.IsAvailable)
                    {
                        output.Apply(signalCopy);
                    }
                });
            }
        }

        public void ResetAll()
        {
            foreach (var registration in outputs)
            {
                if (!registration.Gate.CanAttempt(Time.unscaledTime))
                {
                    continue;
                }

                TryInvoke(registration, registration.Output.ResetDevice);
            }
        }

        public void OnRunReset()
        {
            ResetAll();
        }

        public void OnRunStateChanged(PrototypeRunState previous, PrototypeRunState current)
        {
        }

        public DeviceHealthSnapshot GetHealthSnapshot()
        {
            var available = 0;
            var faulted = 0;
            foreach (var registration in outputs)
            {
                if (registration.Gate.IsFaulted)
                {
                    faulted++;
                }
                else if (registration.Output.IsAvailable)
                {
                    available++;
                }
            }

            return new DeviceHealthSnapshot(outputs.Count, available, faulted);
        }

        private void RebuildRegistrations()
        {
            outputs.Clear();
            if (outputBehaviours == null || outputBehaviours.Length == 0)
            {
                outputBehaviours = GetComponentsInChildren<MonoBehaviour>(true);
            }

            foreach (var behaviour in outputBehaviours)
            {
                if (behaviour is IFeedbackOutput output)
                {
                    outputs.Add(new OutputRegistration(
                        output,
                        new DeviceFaultGate(failureThreshold, retryDelaySeconds)));
                }
            }
        }

        private void TryInvoke(OutputRegistration registration, Action operation)
        {
            try
            {
                operation();
                registration.Gate.MarkSuccess();
            }
            catch (Exception exception)
            {
                var wasFaulted = registration.Gate.IsFaulted;
                registration.Gate.MarkFailure(Time.unscaledTime);
                if (!wasFaulted && registration.Gate.IsFaulted)
                {
                    Debug.LogWarning(
                        $"Feedback output '{registration.Output.DeviceId}' was isolated after repeated failures. " +
                        $"The game will continue and retry after {retryDelaySeconds:0.0}s. {exception.Message}",
                        this);
                }
            }
        }

        private sealed class OutputRegistration
        {
            public OutputRegistration(IFeedbackOutput output, DeviceFaultGate gate)
            {
                Output = output;
                Gate = gate;
            }

            public IFeedbackOutput Output { get; }
            public DeviceFaultGate Gate { get; }
        }
    }
}
