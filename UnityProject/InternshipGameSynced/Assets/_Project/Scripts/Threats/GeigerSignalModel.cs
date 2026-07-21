using System;
using UnityEngine;

namespace InternshipGame.Prototype
{
    public readonly struct GeigerReading
    {
        public GeigerReading(
            float truthClicksPerSecond,
            float reportedClicksPerSecond,
            float jitterMultiplier,
            bool interferenceActive,
            bool falseBurstActive)
        {
            TruthClicksPerSecond = truthClicksPerSecond;
            ReportedClicksPerSecond = reportedClicksPerSecond;
            JitterMultiplier = jitterMultiplier;
            InterferenceActive = interferenceActive;
            FalseBurstActive = falseBurstActive;
        }

        public float TruthClicksPerSecond { get; }
        public float ReportedClicksPerSecond { get; }
        public float JitterMultiplier { get; }
        public bool InterferenceActive { get; }
        public bool FalseBurstActive { get; }
    }

    public sealed class GeigerSignalModel
    {
        public const float OwlInterferenceDistance = 8f;
        public const float MaximumJitterFraction = 0.20f;
        public const float FalseBurstDurationSeconds = 1f;
        public const float FalseBurstCooldownSeconds = 12f;
        public const float FalseBurstClicksPerSecond = 2f;
        public const float MinimumTruthClicksPerSecond = 0.25f;
        public const float MaximumTruthClicksPerSecond = 12f;
        public const float LiveRodDetectionDistance = 30f;

        private readonly int seed;
        private float elapsedSeconds;
        private float falseBurstRemaining;
        private float nextFalseBurstEligibleTime;

        public GeigerSignalModel(int seed, bool interferenceEnabled = true)
        {
            this.seed = seed;
            InterferenceEnabled = interferenceEnabled;
            Reset();
        }

        public bool InterferenceEnabled { get; set; }
        public float ElapsedSeconds => elapsedSeconds;
        public float FalseBurstSecondsRemaining => falseBurstRemaining;

        public GeigerReading Tick(
            float deltaSeconds,
            float nearestLiveRodDistance,
            bool owlActive,
            float owlDistance)
        {
            ValidateDelta(deltaSeconds);
            ValidateDistance(nearestLiveRodDistance, nameof(nearestLiveRodDistance));
            ValidateDistance(owlDistance, nameof(owlDistance));

            elapsedSeconds += deltaSeconds;
            var truthRate = CalculateTruthClicksPerSecond(nearestLiveRodDistance);
            var interferenceActive = InterferenceEnabled &&
                                     owlActive &&
                                     owlDistance <= OwlInterferenceDistance;

            if (!interferenceActive)
            {
                falseBurstRemaining = 0f;
                return new GeigerReading(truthRate, truthRate, 1f, false, false);
            }

            var jitterBucket = unchecked((uint)Mathf.FloorToInt(elapsedSeconds * 4f));
            var jitterUnit = Sample01(0xA341316Cu, jitterBucket);
            var jitterMultiplier = Mathf.Lerp(
                1f - MaximumJitterFraction,
                1f + MaximumJitterFraction,
                jitterUnit);

            falseBurstRemaining = Mathf.Max(0f, falseBurstRemaining - deltaSeconds);
            if (falseBurstRemaining <= 0f && elapsedSeconds >= nextFalseBurstEligibleTime)
            {
                falseBurstRemaining = FalseBurstDurationSeconds;
                nextFalseBurstEligibleTime = elapsedSeconds + FalseBurstCooldownSeconds;
            }

            var falseBurstActive = falseBurstRemaining > 0f;
            var reportedRate = truthRate * jitterMultiplier;
            if (falseBurstActive)
            {
                reportedRate += FalseBurstClicksPerSecond;
            }

            return new GeigerReading(
                truthRate,
                reportedRate,
                jitterMultiplier,
                true,
                falseBurstActive);
        }

        public void Reset()
        {
            elapsedSeconds = 0f;
            falseBurstRemaining = 0f;
            nextFalseBurstEligibleTime = Sample01(0xC8013EA4u, 0u) * FalseBurstCooldownSeconds;
        }

        public static float CalculateTruthClicksPerSecond(float nearestLiveRodDistance)
        {
            ValidateDistance(nearestLiveRodDistance, nameof(nearestLiveRodDistance));
            if (float.IsPositiveInfinity(nearestLiveRodDistance))
            {
                return MinimumTruthClicksPerSecond;
            }

            var proximity = 1f - Mathf.Clamp01(nearestLiveRodDistance / LiveRodDetectionDistance);
            return Mathf.Lerp(
                MinimumTruthClicksPerSecond,
                MaximumTruthClicksPerSecond,
                proximity * proximity);
        }

        private float Sample01(uint channel, uint sampleIndex)
        {
            var value = unchecked((uint)seed);
            value ^= channel * 0x9E3779B9u;
            value ^= sampleIndex * 0x85EBCA6Bu;
            value ^= value >> 16;
            value *= 0x7FEB352Du;
            value ^= value >> 15;
            value *= 0x846CA68Bu;
            value ^= value >> 16;
            return (value & 0x00FFFFFFu) / 16777216f;
        }

        private static void ValidateDelta(float deltaSeconds)
        {
            if (float.IsNaN(deltaSeconds) || float.IsInfinity(deltaSeconds) || deltaSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds));
            }
        }

        private static void ValidateDistance(float distance, string parameterName)
        {
            if (float.IsNaN(distance) || float.IsNegativeInfinity(distance) || distance < 0f)
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }
    }
}
