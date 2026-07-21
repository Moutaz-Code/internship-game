using System;
using System.Collections.Generic;
using UnityEngine;

namespace InternshipGame.Prototype
{
    public sealed class BiometricHub : MonoBehaviour, IPrototypeStatusProvider
    {
        [SerializeField] private MonoBehaviour[] sensorBehaviours = Array.Empty<MonoBehaviour>();
        [SerializeField, Min(0.1f)] private float pollIntervalSeconds = 0.5f;
        [SerializeField, Min(1)] private int failureThreshold = 3;
        [SerializeField, Min(0f)] private float retryDelaySeconds = 5f;

        private readonly List<SensorRegistration> sensors = new();
        private float nextPollTime;

        public bool HasSample { get; private set; }
        public BiometricSample LastSample { get; private set; }
        public string StatusLabel => HasSample
            ? $"Heart rate: {LastSample.HeartRateBpm:0} bpm"
            : $"Biometrics: optional ({AvailableSensorCount}/{sensors.Count} ready)";

        private int AvailableSensorCount
        {
            get
            {
                var count = 0;
                foreach (var registration in sensors)
                {
                    if (!registration.Gate.IsFaulted && registration.Sensor.IsAvailable)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public void Configure(params MonoBehaviour[] configuredSensors)
        {
            sensorBehaviours = configuredSensors ?? Array.Empty<MonoBehaviour>();
        }

        private void Awake()
        {
            sensors.Clear();
            if (sensorBehaviours == null || sensorBehaviours.Length == 0)
            {
                sensorBehaviours = GetComponentsInChildren<MonoBehaviour>(true);
            }

            foreach (var behaviour in sensorBehaviours)
            {
                if (behaviour is IBiometricSensor sensor)
                {
                    sensors.Add(new SensorRegistration(
                        sensor,
                        new DeviceFaultGate(failureThreshold, retryDelaySeconds)));
                }
            }
        }

        private void Update()
        {
            if (Time.unscaledTime < nextPollTime)
            {
                return;
            }

            nextPollTime = Time.unscaledTime + pollIntervalSeconds;
            foreach (var registration in sensors)
            {
                if (!registration.Gate.CanAttempt(Time.unscaledTime) || !registration.Sensor.IsAvailable)
                {
                    continue;
                }

                try
                {
                    if (registration.Gate.IsFaulted)
                    {
                        registration.Sensor.ResetDevice();
                    }

                    if (registration.Sensor.TryRead(out var sample))
                    {
                        LastSample = sample;
                        HasSample = true;
                    }

                    registration.Gate.MarkSuccess();
                }
                catch (Exception exception)
                {
                    var wasFaulted = registration.Gate.IsFaulted;
                    registration.Gate.MarkFailure(Time.unscaledTime);
                    if (!wasFaulted && registration.Gate.IsFaulted)
                    {
                        Debug.LogWarning(
                            $"Biometric sensor '{registration.Sensor.DeviceId}' was isolated after repeated failures. " +
                            $"Gameplay continues without biometric data. {exception.Message}",
                            this);
                    }
                }
            }
        }

        private sealed class SensorRegistration
        {
            public SensorRegistration(IBiometricSensor sensor, DeviceFaultGate gate)
            {
                Sensor = sensor;
                Gate = gate;
            }

            public IBiometricSensor Sensor { get; }
            public DeviceFaultGate Gate { get; }
        }
    }
}
