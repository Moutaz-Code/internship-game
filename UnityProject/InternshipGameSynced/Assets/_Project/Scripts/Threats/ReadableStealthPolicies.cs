using System;
using UnityEngine;

namespace InternshipGame.Prototype
{
    public readonly struct VisualConfirmationTuning
    {
        public VisualConfirmationTuning(
            float concealedDecayPerSecond,
            float noticeableQuietGainPerSecond,
            float noticeableMovingGainPerSecond,
            float exposedGainPerSecond,
            float sprintGainMultiplier,
            float confirmationThreshold,
            float nearlyStillSpeed)
        {
            ValidateNonNegative(concealedDecayPerSecond, nameof(concealedDecayPerSecond));
            ValidateNonNegative(noticeableQuietGainPerSecond, nameof(noticeableQuietGainPerSecond));
            ValidateNonNegative(noticeableMovingGainPerSecond, nameof(noticeableMovingGainPerSecond));
            ValidateNonNegative(exposedGainPerSecond, nameof(exposedGainPerSecond));
            if (sprintGainMultiplier < 1f || float.IsNaN(sprintGainMultiplier) || float.IsInfinity(sprintGainMultiplier))
            {
                throw new ArgumentOutOfRangeException(nameof(sprintGainMultiplier));
            }

            if (confirmationThreshold <= 0f || confirmationThreshold > 1f ||
                float.IsNaN(confirmationThreshold) || float.IsInfinity(confirmationThreshold))
            {
                throw new ArgumentOutOfRangeException(nameof(confirmationThreshold));
            }

            ValidateNonNegative(nearlyStillSpeed, nameof(nearlyStillSpeed));
            ConcealedDecayPerSecond = concealedDecayPerSecond;
            NoticeableQuietGainPerSecond = noticeableQuietGainPerSecond;
            NoticeableMovingGainPerSecond = noticeableMovingGainPerSecond;
            ExposedGainPerSecond = exposedGainPerSecond;
            SprintGainMultiplier = sprintGainMultiplier;
            ConfirmationThreshold = confirmationThreshold;
            NearlyStillSpeed = nearlyStillSpeed;
        }

        public static VisualConfirmationTuning Default => new(
            concealedDecayPerSecond: 1f,
            noticeableQuietGainPerSecond: 0.45f,
            noticeableMovingGainPerSecond: 0.65f,
            exposedGainPerSecond: 1.35f,
            sprintGainMultiplier: 1.35f,
            confirmationThreshold: 1f,
            nearlyStillSpeed: 0.12f);

        public float ConcealedDecayPerSecond { get; }
        public float NoticeableQuietGainPerSecond { get; }
        public float NoticeableMovingGainPerSecond { get; }
        public float ExposedGainPerSecond { get; }
        public float SprintGainMultiplier { get; }
        public float ConfirmationThreshold { get; }
        public float NearlyStillSpeed { get; }

        private static void ValidateNonNegative(float value, string parameterName)
        {
            if (value < 0f || float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }
    }

    public readonly struct VisualConfirmationSnapshot
    {
        public VisualConfirmationSnapshot(
            VisualExposureState exposure,
            float confirmation,
            float ratePerSecond,
            bool confirmed)
        {
            Exposure = exposure;
            Confirmation = confirmation;
            RatePerSecond = ratePerSecond;
            Confirmed = confirmed;
        }

        public VisualExposureState Exposure { get; }
        public float Confirmation { get; }
        public float RatePerSecond { get; }
        public bool Confirmed { get; }
    }

    public sealed class VisualConfirmationModel
    {
        private readonly VisualConfirmationTuning tuning;

        public VisualConfirmationModel(VisualConfirmationTuning tuning)
        {
            this.tuning = tuning;
            Reset();
        }

        public float Confirmation { get; private set; }
        public VisualExposureState Exposure { get; private set; }
        public float RatePerSecond { get; private set; }
        public bool IsConfirmed => Confirmation >= tuning.ConfirmationThreshold;

        public VisualConfirmationSnapshot Tick(
            int visibleSamples,
            bool isCrouching,
            float planarSpeed,
            bool isSprinting,
            float deltaSeconds)
        {
            if (deltaSeconds < 0f || float.IsNaN(deltaSeconds) || float.IsInfinity(deltaSeconds))
            {
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds));
            }

            Exposure = Classify(visibleSamples);
            RatePerSecond = Exposure switch
            {
                VisualExposureState.Concealed => -tuning.ConcealedDecayPerSecond,
                VisualExposureState.Noticeable when isCrouching || planarSpeed <= tuning.NearlyStillSpeed =>
                    tuning.NoticeableQuietGainPerSecond,
                VisualExposureState.Noticeable => tuning.NoticeableMovingGainPerSecond,
                _ => tuning.ExposedGainPerSecond,
            };
            if (RatePerSecond > 0f && isSprinting)
            {
                RatePerSecond *= tuning.SprintGainMultiplier;
            }

            Confirmation = Mathf.Clamp01(Confirmation + RatePerSecond * deltaSeconds);
            return Snapshot;
        }

        public void Reset()
        {
            Confirmation = 0f;
            Exposure = VisualExposureState.Concealed;
            RatePerSecond = 0f;
        }

        public VisualConfirmationSnapshot Snapshot => new(
            Exposure,
            Confirmation,
            RatePerSecond,
            IsConfirmed);

        public static VisualExposureState Classify(int visibleSamples)
        {
            return visibleSamples switch
            {
                <= 0 => VisualExposureState.Concealed,
                1 => VisualExposureState.Noticeable,
                _ => VisualExposureState.Exposed,
            };
        }
    }

    public readonly struct ThreatHearingDecision
    {
        public ThreatHearingDecision(float evaluatedStrength, float threshold)
        {
            EvaluatedStrength = Mathf.Clamp01(evaluatedStrength);
            Threshold = Mathf.Clamp01(threshold);
            Accepted = EvaluatedStrength >= Threshold;
            RejectionReason = Accepted ? string.Empty : "below evaluated hearing threshold";
        }

        public bool Accepted { get; }
        public float EvaluatedStrength { get; }
        public float Threshold { get; }
        public string RejectionReason { get; }
    }

    public static class ThreatHearingPolicy
    {
        public const float DefaultThreshold = 0.15f;

        public static ThreatHearingDecision Evaluate(
            ThreatStimulus stimulus,
            Vector3 listenerPosition,
            float threshold = DefaultThreshold)
        {
            return new ThreatHearingDecision(
                stimulus.EvaluateAt(listenerPosition),
                threshold);
        }
    }

    public readonly struct ThreatTargetDecision
    {
        public ThreatTargetDecision(ThreatKnowledgeSource source, Vector3 position)
        {
            Source = source;
            Position = position;
        }

        public ThreatKnowledgeSource Source { get; }
        public Vector3 Position { get; }
    }

    public static class ThreatKnowledgePolicy
    {
        public static ThreatTargetDecision Select(
            bool hasConfirmedCurrentVisual,
            Vector3 currentVisualPosition,
            bool hasAcceptedSound,
            Vector3 acceptedSoundPosition,
            bool hasLastConfirmedVisual,
            Vector3 lastConfirmedVisualPosition,
            bool hasLocalSearchProbe,
            Vector3 localSearchProbePosition,
            Vector3 patrolPosition)
        {
            if (hasConfirmedCurrentVisual)
            {
                return new ThreatTargetDecision(
                    ThreatKnowledgeSource.ConfirmedCurrentVisual,
                    currentVisualPosition);
            }

            if (hasAcceptedSound)
            {
                return new ThreatTargetDecision(
                    ThreatKnowledgeSource.AcceptedSound,
                    acceptedSoundPosition);
            }

            if (hasLastConfirmedVisual)
            {
                return new ThreatTargetDecision(
                    ThreatKnowledgeSource.LastConfirmedVisual,
                    lastConfirmedVisualPosition);
            }

            if (hasLocalSearchProbe)
            {
                return new ThreatTargetDecision(
                    ThreatKnowledgeSource.LocalSearchProbe,
                    localSearchProbePosition);
            }

            return new ThreatTargetDecision(ThreatKnowledgeSource.PatrolTarget, patrolPosition);
        }
    }

    public readonly struct MovementNoiseProfile
    {
        public MovementNoiseProfile(float pulseInterval, float intensity, float radius)
        {
            PulseInterval = pulseInterval;
            Intensity = intensity;
            Radius = radius;
        }

        public float PulseInterval { get; }
        public float Intensity { get; }
        public float Radius { get; }
    }

    public static class PlayerMovementNoisePolicy
    {
        public const float DefaultStationarySpeed = 0.08f;

        public static MovementNoiseBand Classify(
            float planarSpeed,
            bool isCrouching,
            bool isSprinting,
            float stationarySpeed = DefaultStationarySpeed)
        {
            if (planarSpeed <= Mathf.Max(0f, stationarySpeed))
            {
                return MovementNoiseBand.Still;
            }

            if (isCrouching)
            {
                return MovementNoiseBand.Crouch;
            }

            return isSprinting ? MovementNoiseBand.Sprint : MovementNoiseBand.Walk;
        }

        public static MovementNoiseProfile GetProfile(MovementNoiseBand band)
        {
            return band switch
            {
                MovementNoiseBand.Crouch => new MovementNoiseProfile(0.80f, 0.16f, 6f),
                MovementNoiseBand.Walk => new MovementNoiseProfile(0.68f, 0.32f, 10f),
                MovementNoiseBand.Sprint => new MovementNoiseProfile(0.45f, 0.62f, 15f),
                _ => new MovementNoiseProfile(0f, 0f, 0f),
            };
        }
    }

    public sealed class MovementNoisePulseScheduler
    {
        private MovementNoiseBand previousBand = MovementNoiseBand.Still;

        public float NextPulseAt { get; private set; }

        public bool TryCreatePulse(
            MovementNoiseBand band,
            float now,
            out MovementNoiseProfile profile)
        {
            profile = PlayerMovementNoisePolicy.GetProfile(band);
            if (band == MovementNoiseBand.Still)
            {
                previousBand = band;
                NextPulseAt = now;
                return false;
            }

            if (previousBand == MovementNoiseBand.Still)
            {
                previousBand = band;
                NextPulseAt = now + profile.PulseInterval;
                return true;
            }

            previousBand = band;
            if (now < NextPulseAt)
            {
                return false;
            }

            NextPulseAt = now + profile.PulseInterval;
            return true;
        }

        public void Reset()
        {
            previousBand = MovementNoiseBand.Still;
            NextPulseAt = 0f;
        }
    }
}
