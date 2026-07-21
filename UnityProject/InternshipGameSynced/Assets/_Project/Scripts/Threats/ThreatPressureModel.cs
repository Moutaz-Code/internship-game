using System;
using UnityEngine;

namespace InternshipGame.Prototype
{
    public readonly struct ThreatPressureSnapshot
    {
        public ThreatPressureSnapshot(
            float totalPressure,
            float timePressure,
            float objectivePressure,
            float noisePressure,
            ThreatWing biasedWing,
            float biasSecondsRemaining)
        {
            TotalPressure = totalPressure;
            TimePressure = timePressure;
            ObjectivePressure = objectivePressure;
            NoisePressure = noisePressure;
            BiasedWing = biasedWing;
            BiasSecondsRemaining = biasSecondsRemaining;
        }

        public float TotalPressure { get; }
        public float TimePressure { get; }
        public float ObjectivePressure { get; }
        public float NoisePressure { get; }
        public ThreatWing BiasedWing { get; }
        public float BiasSecondsRemaining { get; }
    }

    public sealed class ThreatPressureModel : IThreatStimulusSink
    {
        public const float TimeToMaximumPressureSeconds = 240f;
        public const float NoiseDecayPerSecond = 0.12f;
        public const float PatrolBiasDurationSeconds = 60f;
        public const float BiasedPatrolWeight = 1.75f;

        private Vector3 threatPosition;
        private float elapsedSeconds;
        private float objectiveProgress;
        private float noisePressure;
        private ThreatWing lastVisitedWing;
        private int consecutiveWingVisits;
        private ThreatWing biasedWing;
        private float biasSecondsRemaining;

        public float ElapsedSeconds => elapsedSeconds;
        public float ObjectiveProgress => objectiveProgress;
        public float NoisePressure => noisePressure;
        public ThreatWing BiasedWing => biasedWing;
        public ThreatStimulus? LastStimulus { get; private set; }
        public float LastStimulusStrength { get; private set; }

        public ThreatPressureSnapshot Snapshot
        {
            get
            {
                var timePressure = Mathf.Clamp01(elapsedSeconds / TimeToMaximumPressureSeconds);
                var total = Mathf.Clamp01(
                    timePressure * 0.30f +
                    objectiveProgress * 0.35f +
                    noisePressure * 0.45f);
                return new ThreatPressureSnapshot(
                    total,
                    timePressure,
                    objectiveProgress,
                    noisePressure,
                    biasedWing,
                    biasSecondsRemaining);
            }
        }

        public void Tick(float deltaSeconds)
        {
            if (float.IsNaN(deltaSeconds) || float.IsInfinity(deltaSeconds) || deltaSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds));
            }

            elapsedSeconds += deltaSeconds;
            noisePressure = Mathf.MoveTowards(noisePressure, 0f, NoiseDecayPerSecond * deltaSeconds);

            if (biasedWing == ThreatWing.None)
            {
                return;
            }

            biasSecondsRemaining = Mathf.Max(0f, biasSecondsRemaining - deltaSeconds);
            if (biasSecondsRemaining <= 0f)
            {
                biasedWing = ThreatWing.None;
            }
        }

        public void SetThreatPosition(Vector3 position)
        {
            threatPosition = position;
        }

        public void SetObjectiveProgress(float normalizedProgress)
        {
            if (float.IsNaN(normalizedProgress) || float.IsInfinity(normalizedProgress))
            {
                throw new ArgumentOutOfRangeException(nameof(normalizedProgress));
            }

            objectiveProgress = Mathf.Clamp01(normalizedProgress);
        }

        public void ReportStimulus(ThreatStimulus stimulus)
        {
            var spatialStrength = stimulus.EvaluateAt(threatPosition);
            var weightedStrength = spatialStrength * GetStimulusWeight(stimulus.Kind);
            noisePressure = Mathf.Clamp01(noisePressure + weightedStrength);
            LastStimulus = stimulus;
            LastStimulusStrength = spatialStrength;
        }

        public void RecordWingVisit(ThreatWing wing)
        {
            if (wing == ThreatWing.None)
            {
                return;
            }

            if (wing == lastVisitedWing)
            {
                consecutiveWingVisits++;
            }
            else
            {
                lastVisitedWing = wing;
                consecutiveWingVisits = 1;
            }

            if (consecutiveWingVisits < 2)
            {
                return;
            }

            biasedWing = wing;
            biasSecondsRemaining = PatrolBiasDurationSeconds;
        }

        public float GetPatrolWeight(ThreatWing wing)
        {
            return biasedWing != ThreatWing.None && wing == biasedWing
                ? BiasedPatrolWeight
                : 1f;
        }

        public void Reset()
        {
            threatPosition = Vector3.zero;
            elapsedSeconds = 0f;
            objectiveProgress = 0f;
            noisePressure = 0f;
            lastVisitedWing = ThreatWing.None;
            consecutiveWingVisits = 0;
            biasedWing = ThreatWing.None;
            biasSecondsRemaining = 0f;
            LastStimulus = null;
            LastStimulusStrength = 0f;
        }

        private static float GetStimulusWeight(ThreatStimulusKind kind)
        {
            return kind switch
            {
                ThreatStimulusKind.Movement => 0.20f,
                ThreatStimulusKind.Sprint => 0.40f,
                ThreatStimulusKind.JumpLanding => 0.55f,
                ThreatStimulusKind.DroppedProp => 0.70f,
                ThreatStimulusKind.RodRejected => 1f,
                ThreatStimulusKind.ObjectiveProgress => 0.60f,
                _ => 0f,
            };
        }
    }
}
