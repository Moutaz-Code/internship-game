using System;
using System.Collections.Generic;
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

    public readonly struct RouteAdaptationTuning
    {
        public RouteAdaptationTuning(
            float directEvidenceWeight,
            float inferredEvidenceWeight,
            float suspiciousThreshold,
            float adaptedThreshold,
            float inferredEvidenceCooldownSeconds,
            float confidenceDecayDelaySeconds,
            float confidenceDecayPerSecond,
            float crossWingReductionMultiplier,
            float suspiciousPatrolWeight,
            float adaptedPatrolWeight,
            float patrolBiasDurationSeconds,
            float counterAdaptedDurationSeconds,
            int maximumConsecutiveBiasedSelections)
        {
            if (directEvidenceWeight <= 0f || float.IsNaN(directEvidenceWeight) || float.IsInfinity(directEvidenceWeight))
            {
                throw new ArgumentOutOfRangeException(nameof(directEvidenceWeight));
            }

            if (inferredEvidenceWeight <= 0f || float.IsNaN(inferredEvidenceWeight) || float.IsInfinity(inferredEvidenceWeight))
            {
                throw new ArgumentOutOfRangeException(nameof(inferredEvidenceWeight));
            }

            if (suspiciousThreshold <= 0f || adaptedThreshold <= suspiciousThreshold)
            {
                throw new ArgumentOutOfRangeException(nameof(adaptedThreshold));
            }

            ValidateNonNegative(inferredEvidenceCooldownSeconds, nameof(inferredEvidenceCooldownSeconds));
            ValidateNonNegative(confidenceDecayDelaySeconds, nameof(confidenceDecayDelaySeconds));
            ValidateNonNegative(confidenceDecayPerSecond, nameof(confidenceDecayPerSecond));
            ValidateNonNegative(crossWingReductionMultiplier, nameof(crossWingReductionMultiplier));
            ValidateNonNegative(counterAdaptedDurationSeconds, nameof(counterAdaptedDurationSeconds));

            if (suspiciousPatrolWeight < 1f || float.IsNaN(suspiciousPatrolWeight) || float.IsInfinity(suspiciousPatrolWeight))
            {
                throw new ArgumentOutOfRangeException(nameof(suspiciousPatrolWeight));
            }

            if (adaptedPatrolWeight < 1f || float.IsNaN(adaptedPatrolWeight) || float.IsInfinity(adaptedPatrolWeight))
            {
                throw new ArgumentOutOfRangeException(nameof(adaptedPatrolWeight));
            }

            if (patrolBiasDurationSeconds <= 0f || float.IsNaN(patrolBiasDurationSeconds) || float.IsInfinity(patrolBiasDurationSeconds))
            {
                throw new ArgumentOutOfRangeException(nameof(patrolBiasDurationSeconds));
            }

            if (maximumConsecutiveBiasedSelections < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumConsecutiveBiasedSelections));
            }

            DirectEvidenceWeight = directEvidenceWeight;
            InferredEvidenceWeight = inferredEvidenceWeight;
            SuspiciousThreshold = suspiciousThreshold;
            AdaptedThreshold = adaptedThreshold;
            InferredEvidenceCooldownSeconds = inferredEvidenceCooldownSeconds;
            ConfidenceDecayDelaySeconds = confidenceDecayDelaySeconds;
            ConfidenceDecayPerSecond = confidenceDecayPerSecond;
            CrossWingReductionMultiplier = crossWingReductionMultiplier;
            SuspiciousPatrolWeight = Mathf.Min(suspiciousPatrolWeight, ThreatPressureModel.BiasedPatrolWeight);
            AdaptedPatrolWeight = Mathf.Max(
                SuspiciousPatrolWeight,
                Mathf.Min(adaptedPatrolWeight, ThreatPressureModel.BiasedPatrolWeight));
            PatrolBiasDurationSeconds = patrolBiasDurationSeconds;
            CounterAdaptedDurationSeconds = counterAdaptedDurationSeconds;
            MaximumConsecutiveBiasedSelections = maximumConsecutiveBiasedSelections;
        }

        public static RouteAdaptationTuning Default => new(
            directEvidenceWeight: 1f,
            inferredEvidenceWeight: 0.5f,
            suspiciousThreshold: 1f,
            adaptedThreshold: 2f,
            inferredEvidenceCooldownSeconds: 4f,
            confidenceDecayDelaySeconds: 60f,
            confidenceDecayPerSecond: 0.025f,
            crossWingReductionMultiplier: 1f,
            suspiciousPatrolWeight: 1.25f,
            adaptedPatrolWeight: ThreatPressureModel.BiasedPatrolWeight,
            patrolBiasDurationSeconds: ThreatPressureModel.PatrolBiasDurationSeconds,
            counterAdaptedDurationSeconds: 12f,
            maximumConsecutiveBiasedSelections: 2);

        public float DirectEvidenceWeight { get; }
        public float InferredEvidenceWeight { get; }
        public float SuspiciousThreshold { get; }
        public float AdaptedThreshold { get; }
        public float InferredEvidenceCooldownSeconds { get; }
        public float ConfidenceDecayDelaySeconds { get; }
        public float ConfidenceDecayPerSecond { get; }
        public float CrossWingReductionMultiplier { get; }
        public float SuspiciousPatrolWeight { get; }
        public float AdaptedPatrolWeight { get; }
        public float PatrolBiasDurationSeconds { get; }
        public float CounterAdaptedDurationSeconds { get; }
        public int MaximumConsecutiveBiasedSelections { get; }

        private static void ValidateNonNegative(float value, string parameterName)
        {
            if (value < 0f || float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentOutOfRangeException(parameterName);
            }
        }
    }

    public readonly struct RouteAdaptationSnapshot
    {
        public RouteAdaptationSnapshot(
            bool enabled,
            RouteAdaptationState state,
            float westConfidence,
            float centralConfidence,
            float eastConfidence,
            ThreatWing dominantWing,
            ThreatWing preferredPatrolWing,
            float biasSecondsRemaining,
            float secondsUntilDecay,
            float counterAdaptedSecondsRemaining,
            RouteEvidence? lastEvidence,
            RouteAdaptationState lastTransitionFrom,
            RouteAdaptationState lastTransitionTo,
            float lastTransitionAtSeconds,
            RouteAdaptationResponse selectedResponse,
            int resetSequence,
            string lastResetReason)
        {
            Enabled = enabled;
            State = state;
            WestConfidence = westConfidence;
            CentralConfidence = centralConfidence;
            EastConfidence = eastConfidence;
            DominantWing = dominantWing;
            PreferredPatrolWing = preferredPatrolWing;
            BiasSecondsRemaining = biasSecondsRemaining;
            SecondsUntilDecay = secondsUntilDecay;
            CounterAdaptedSecondsRemaining = counterAdaptedSecondsRemaining;
            LastEvidence = lastEvidence;
            LastTransitionFrom = lastTransitionFrom;
            LastTransitionTo = lastTransitionTo;
            LastTransitionAtSeconds = lastTransitionAtSeconds;
            SelectedResponse = selectedResponse;
            ResetSequence = resetSequence;
            LastResetReason = lastResetReason ?? string.Empty;
        }

        public bool Enabled { get; }
        public RouteAdaptationState State { get; }
        public float WestConfidence { get; }
        public float CentralConfidence { get; }
        public float EastConfidence { get; }
        public ThreatWing DominantWing { get; }
        public ThreatWing PreferredPatrolWing { get; }
        public float BiasSecondsRemaining { get; }
        public float SecondsUntilDecay { get; }
        public float CounterAdaptedSecondsRemaining { get; }
        public RouteEvidence? LastEvidence { get; }
        public RouteAdaptationState LastTransitionFrom { get; }
        public RouteAdaptationState LastTransitionTo { get; }
        public float LastTransitionAtSeconds { get; }
        public RouteAdaptationResponse SelectedResponse { get; }
        public int ResetSequence { get; }
        public string LastResetReason { get; }
    }

    public readonly struct ThreatPatrolSelection
    {
        public ThreatPatrolSelection(int index, ThreatWing wing, bool usedPreference, bool forcedVariety)
        {
            Index = index;
            Wing = wing;
            UsedPreference = usedPreference;
            ForcedVariety = forcedVariety;
        }

        public int Index { get; }
        public ThreatWing Wing { get; }
        public bool UsedPreference { get; }
        public bool ForcedVariety { get; }
    }

    public static class ThreatPatrolSelectionPolicy
    {
        public static ThreatPatrolSelection SelectNext(
            IReadOnlyList<ThreatWing> patrolWings,
            int currentIndex,
            ThreatWing preferredWing,
            bool forceOutsidePreferredWing)
        {
            if (patrolWings == null || patrolWings.Count == 0)
            {
                return new ThreatPatrolSelection(-1, ThreatWing.None, false, false);
            }

            var normalizedCurrent = Mathf.Clamp(currentIndex, 0, patrolWings.Count - 1);
            if (preferredWing != ThreatWing.None)
            {
                var preferredIndex = FindNext(
                    patrolWings,
                    normalizedCurrent,
                    wing => forceOutsidePreferredWing ? wing != preferredWing : wing == preferredWing);
                if (preferredIndex >= 0)
                {
                    var selectedWing = patrolWings[preferredIndex];
                    return new ThreatPatrolSelection(
                        preferredIndex,
                        selectedWing,
                        !forceOutsidePreferredWing && selectedWing == preferredWing,
                        forceOutsidePreferredWing && selectedWing != preferredWing);
                }
            }

            var fallbackIndex = FindNext(patrolWings, normalizedCurrent, _ => true);
            return fallbackIndex >= 0
                ? new ThreatPatrolSelection(fallbackIndex, patrolWings[fallbackIndex], false, false)
                : new ThreatPatrolSelection(-1, ThreatWing.None, false, false);
        }

        private static int FindNext(
            IReadOnlyList<ThreatWing> patrolWings,
            int currentIndex,
            Func<ThreatWing, bool> predicate)
        {
            for (var offset = 1; offset <= patrolWings.Count; offset++)
            {
                var candidateIndex = (currentIndex + offset) % patrolWings.Count;
                var wing = patrolWings[candidateIndex];
                if (wing != ThreatWing.None && predicate(wing))
                {
                    return candidateIndex;
                }
            }

            return -1;
        }
    }

    public sealed class ThreatPressureModel : IThreatStimulusSink
    {
        public const float TimeToMaximumPressureSeconds = 240f;
        public const float NoiseDecayPerSecond = 0.12f;
        public const float PatrolBiasDurationSeconds = 60f;
        public const float BiasedPatrolWeight = 1.75f;

        private readonly RouteAdaptationTuning routeTuning;
        private readonly float[] routeConfidence = new float[3];
        private readonly float[] lastRouteEvidenceAt = new float[3];
        private readonly float[] nextInferredEvidenceAt = new float[3];
        private Vector3 threatPosition;
        private float elapsedSeconds;
        private float objectiveProgress;
        private float noisePressure;
        private RouteAdaptationState adaptationState = RouteAdaptationState.Uncertain;
        private ThreatWing dominantWing;
        private float biasSecondsRemaining;
        private float counterAdaptedSecondsRemaining;
        private int consecutiveBiasedPatrolSelections;

        public ThreatPressureModel()
            : this(RouteAdaptationTuning.Default)
        {
        }

        public ThreatPressureModel(RouteAdaptationTuning tuning)
        {
            if (tuning.AdaptedThreshold <= tuning.SuspiciousThreshold)
            {
                throw new ArgumentException("Route adaptation tuning is not initialized.", nameof(tuning));
            }

            routeTuning = tuning;
            for (var i = 0; i < lastRouteEvidenceAt.Length; i++)
            {
                lastRouteEvidenceAt[i] = float.NegativeInfinity;
            }
        }

        public float ElapsedSeconds => elapsedSeconds;
        public float ObjectiveProgress => objectiveProgress;
        public float NoisePressure => noisePressure;
        public ThreatWing BiasedWing =>
            AdaptationEnabled && adaptationState == RouteAdaptationState.Adapted && biasSecondsRemaining > 0f
                ? dominantWing
                : ThreatWing.None;
        public ThreatWing PreferredPatrolWing =>
            AdaptationEnabled &&
            (adaptationState == RouteAdaptationState.Suspicious ||
             (adaptationState == RouteAdaptationState.Adapted && biasSecondsRemaining > 0f))
                ? dominantWing
                : ThreatWing.None;
        public bool AdaptationEnabled { get; private set; } = true;
        public bool RequiresPatrolVariety =>
            PreferredPatrolWing != ThreatWing.None &&
            consecutiveBiasedPatrolSelections >= routeTuning.MaximumConsecutiveBiasedSelections;
        public int ConsecutiveBiasedPatrolSelections => consecutiveBiasedPatrolSelections;
        public RouteAdaptationTuning RouteTuning => routeTuning;
        public RouteEvidence? LastRouteEvidence { get; private set; }
        public ThreatStimulus? LastStimulus { get; private set; }
        public float LastStimulusStrength { get; private set; }
        public RouteAdaptationState LastTransitionFrom { get; private set; } = RouteAdaptationState.Uncertain;
        public RouteAdaptationState LastTransitionTo { get; private set; } = RouteAdaptationState.Uncertain;
        public float LastTransitionAtSeconds { get; private set; }
        public RouteAdaptationResponse SelectedResponse { get; private set; } = RouteAdaptationResponse.BaselinePatrol;
        public int ResetSequence { get; private set; }
        public string LastResetReason { get; private set; } = "initial";

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
                    BiasedWing,
                    biasSecondsRemaining);
            }
        }

        public RouteAdaptationSnapshot Adaptation
        {
            get
            {
                return new RouteAdaptationSnapshot(
                    AdaptationEnabled,
                    adaptationState,
                    GetRouteConfidence(ThreatWing.West),
                    GetRouteConfidence(ThreatWing.Central),
                    GetRouteConfidence(ThreatWing.East),
                    dominantWing,
                    PreferredPatrolWing,
                    biasSecondsRemaining,
                    GetSecondsUntilDecay(dominantWing),
                    counterAdaptedSecondsRemaining,
                    LastRouteEvidence,
                    LastTransitionFrom,
                    LastTransitionTo,
                    LastTransitionAtSeconds,
                    SelectedResponse,
                    ResetSequence,
                    LastResetReason);
            }
        }

        public void Tick(float deltaSeconds)
        {
            if (float.IsNaN(deltaSeconds) || float.IsInfinity(deltaSeconds) || deltaSeconds < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds));
            }

            var previousElapsed = elapsedSeconds;
            elapsedSeconds += deltaSeconds;
            noisePressure = Mathf.MoveTowards(noisePressure, 0f, NoiseDecayPerSecond * deltaSeconds);

            if (!AdaptationEnabled)
            {
                return;
            }

            biasSecondsRemaining = Mathf.Max(0f, biasSecondsRemaining - deltaSeconds);
            counterAdaptedSecondsRemaining = Mathf.Max(0f, counterAdaptedSecondsRemaining - deltaSeconds);

            for (var i = 0; i < routeConfidence.Length; i++)
            {
                if (float.IsNegativeInfinity(lastRouteEvidenceAt[i]) || routeConfidence[i] <= 0f)
                {
                    continue;
                }

                var decayStartsAt = lastRouteEvidenceAt[i] + routeTuning.ConfidenceDecayDelaySeconds;
                var decayDuration = Mathf.Max(0f, elapsedSeconds - Mathf.Max(previousElapsed, decayStartsAt));
                routeConfidence[i] = Mathf.Max(
                    0f,
                    routeConfidence[i] - routeTuning.ConfidenceDecayPerSecond * decayDuration);
            }

            var previousPreferredWing = PreferredPatrolWing;
            dominantWing = FindDominantWing(LastRouteEvidence?.Wing ?? ThreatWing.None);
            if (counterAdaptedSecondsRemaining > 0f)
            {
                SetAdaptationState(RouteAdaptationState.CounterAdapted);
                biasSecondsRemaining = 0f;
            }
            else
            {
                RefreshStateFromConfidence(refreshBiasWindow: false);
            }

            if (previousPreferredWing != PreferredPatrolWing)
            {
                consecutiveBiasedPatrolSelections = 0;
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

        public bool RecordRouteEvidence(RouteEvidence evidence)
        {
            if (!AdaptationEnabled || evidence.Wing == ThreatWing.None)
            {
                return false;
            }

            var evidenceIndex = ToIndex(evidence.Wing);
            if (evidence.Source == RouteEvidenceSource.Inferred && elapsedSeconds < nextInferredEvidenceAt[evidenceIndex])
            {
                return false;
            }

            var previousPreferredWing = PreferredPatrolWing;
            var previousDominantWing = dominantWing;
            var previousDominantConfidence = GetRouteConfidence(previousDominantWing);
            var weight = evidence.Source == RouteEvidenceSource.Direct
                ? routeTuning.DirectEvidenceWeight
                : routeTuning.InferredEvidenceWeight;
            var reduction = weight * routeTuning.CrossWingReductionMultiplier;
            var reducedPreviousPrediction = false;

            for (var i = 0; i < routeConfidence.Length; i++)
            {
                if (i == evidenceIndex || routeConfidence[i] <= 0f)
                {
                    continue;
                }

                var before = routeConfidence[i];
                routeConfidence[i] = Mathf.Max(0f, before - reduction);
                reducedPreviousPrediction |=
                    FromIndex(i) == previousDominantWing &&
                    before >= routeTuning.SuspiciousThreshold &&
                    routeConfidence[i] < before;
            }

            routeConfidence[evidenceIndex] = Mathf.Min(
                routeTuning.AdaptedThreshold,
                routeConfidence[evidenceIndex] + weight);
            lastRouteEvidenceAt[evidenceIndex] = elapsedSeconds;
            if (evidence.Source == RouteEvidenceSource.Inferred)
            {
                nextInferredEvidenceAt[evidenceIndex] = elapsedSeconds + routeTuning.InferredEvidenceCooldownSeconds;
            }

            LastRouteEvidence = evidence;
            dominantWing = FindDominantWing(evidence.Wing);
            var counteredOldPrediction =
                reducedPreviousPrediction &&
                previousDominantWing != ThreatWing.None &&
                previousDominantWing != evidence.Wing &&
                previousDominantConfidence >= routeTuning.SuspiciousThreshold;

            if (counteredOldPrediction)
            {
                counterAdaptedSecondsRemaining = routeTuning.CounterAdaptedDurationSeconds;
                biasSecondsRemaining = 0f;
                SetAdaptationState(RouteAdaptationState.CounterAdapted);
            }
            else
            {
                counterAdaptedSecondsRemaining = 0f;
                RefreshStateFromConfidence(refreshBiasWindow: true);
            }

            if (previousPreferredWing != PreferredPatrolWing)
            {
                consecutiveBiasedPatrolSelections = 0;
            }

            return true;
        }

        public float GetRouteConfidence(ThreatWing wing)
        {
            return wing == ThreatWing.None ? 0f : routeConfidence[ToIndex(wing)];
        }

        public float GetPatrolWeight(ThreatWing wing)
        {
            if (!AdaptationEnabled || wing == ThreatWing.None || wing != dominantWing)
            {
                return 1f;
            }

            return adaptationState switch
            {
                RouteAdaptationState.Suspicious => routeTuning.SuspiciousPatrolWeight,
                RouteAdaptationState.Adapted when biasSecondsRemaining > 0f => routeTuning.AdaptedPatrolWeight,
                _ => 1f,
            };
        }

        public void RecordPatrolSelection(ThreatWing selectedWing, RouteAdaptationResponse response)
        {
            var preferredWing = PreferredPatrolWing;
            if (preferredWing != ThreatWing.None && selectedWing == preferredWing)
            {
                consecutiveBiasedPatrolSelections++;
            }
            else
            {
                consecutiveBiasedPatrolSelections = 0;
            }

            SelectedResponse = AdaptationEnabled ? response : RouteAdaptationResponse.BaselinePatrol;
        }

        public void SetSelectedResponse(RouteAdaptationResponse response)
        {
            SelectedResponse = AdaptationEnabled ? response : RouteAdaptationResponse.BaselinePatrol;
        }

        public void SetAdaptationEnabled(bool enabled)
        {
            if (AdaptationEnabled == enabled)
            {
                return;
            }

            AdaptationEnabled = enabled;
            ClearRouteMemory(enabled ? "F3 enabled" : "F3 disabled");
        }

        public void Reset()
        {
            threatPosition = Vector3.zero;
            elapsedSeconds = 0f;
            objectiveProgress = 0f;
            noisePressure = 0f;
            LastStimulus = null;
            LastStimulusStrength = 0f;
            ClearRouteMemory("run reset");
        }

        private void ClearRouteMemory(string reason)
        {
            for (var i = 0; i < routeConfidence.Length; i++)
            {
                routeConfidence[i] = 0f;
                lastRouteEvidenceAt[i] = float.NegativeInfinity;
                nextInferredEvidenceAt[i] = 0f;
            }

            adaptationState = RouteAdaptationState.Uncertain;
            dominantWing = ThreatWing.None;
            biasSecondsRemaining = 0f;
            counterAdaptedSecondsRemaining = 0f;
            consecutiveBiasedPatrolSelections = 0;
            LastRouteEvidence = null;
            LastTransitionFrom = RouteAdaptationState.Uncertain;
            LastTransitionTo = RouteAdaptationState.Uncertain;
            LastTransitionAtSeconds = elapsedSeconds;
            SelectedResponse = RouteAdaptationResponse.BaselinePatrol;
            ResetSequence++;
            LastResetReason = reason ?? string.Empty;
        }

        private void RefreshStateFromConfidence(bool refreshBiasWindow)
        {
            var dominantConfidence = GetRouteConfidence(dominantWing);
            var nextState = dominantConfidence >= routeTuning.AdaptedThreshold
                ? RouteAdaptationState.Adapted
                : dominantConfidence >= routeTuning.SuspiciousThreshold
                    ? RouteAdaptationState.Suspicious
                    : RouteAdaptationState.Uncertain;
            var wasAdapted = adaptationState == RouteAdaptationState.Adapted;
            SetAdaptationState(nextState);

            if (nextState == RouteAdaptationState.Adapted)
            {
                if (refreshBiasWindow || !wasAdapted)
                {
                    biasSecondsRemaining = routeTuning.PatrolBiasDurationSeconds;
                }
            }
            else
            {
                biasSecondsRemaining = 0f;
            }
        }

        private void SetAdaptationState(RouteAdaptationState nextState)
        {
            if (adaptationState == nextState)
            {
                return;
            }

            LastTransitionFrom = adaptationState;
            LastTransitionTo = nextState;
            LastTransitionAtSeconds = elapsedSeconds;
            adaptationState = nextState;
            SelectedResponse = nextState switch
            {
                RouteAdaptationState.Suspicious => RouteAdaptationResponse.SuspiciousRouteInspection,
                RouteAdaptationState.Adapted => RouteAdaptationResponse.AdaptedRouteBias,
                RouteAdaptationState.CounterAdapted => RouteAdaptationResponse.CounterAdaptationOpening,
                _ => RouteAdaptationResponse.BaselinePatrol,
            };
        }

        private ThreatWing FindDominantWing(ThreatWing tiePreference)
        {
            var bestWing = ThreatWing.None;
            var bestConfidence = 0f;
            for (var i = 0; i < routeConfidence.Length; i++)
            {
                var wing = FromIndex(i);
                var confidence = routeConfidence[i];
                if (confidence > bestConfidence + 0.0001f ||
                    (confidence > 0f && Mathf.Abs(confidence - bestConfidence) <= 0.0001f && wing == tiePreference))
                {
                    bestWing = wing;
                    bestConfidence = confidence;
                }
            }

            return bestWing;
        }

        private float GetSecondsUntilDecay(ThreatWing wing)
        {
            if (wing == ThreatWing.None)
            {
                return 0f;
            }

            var lastEvidenceAt = lastRouteEvidenceAt[ToIndex(wing)];
            return float.IsNegativeInfinity(lastEvidenceAt)
                ? 0f
                : Mathf.Max(0f, lastEvidenceAt + routeTuning.ConfidenceDecayDelaySeconds - elapsedSeconds);
        }

        private static int ToIndex(ThreatWing wing)
        {
            return wing switch
            {
                ThreatWing.West => 0,
                ThreatWing.Central => 1,
                ThreatWing.East => 2,
                _ => throw new ArgumentOutOfRangeException(nameof(wing)),
            };
        }

        private static ThreatWing FromIndex(int index)
        {
            return index switch
            {
                0 => ThreatWing.West,
                1 => ThreatWing.Central,
                2 => ThreatWing.East,
                _ => ThreatWing.None,
            };
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

    public static class ThreatRouteEvidencePolicy
    {
        public static bool TryRecordDirectObservation(
            ThreatPressureModel model,
            ThreatWing wing,
            bool threatIsActive,
            bool hasLineOfSight,
            string zoneLabel,
            Vector3 observedPosition)
        {
            if (model == null || wing == ThreatWing.None || !threatIsActive || !hasLineOfSight)
            {
                return false;
            }

            return model.RecordRouteEvidence(new RouteEvidence(
                RouteEvidenceSource.Direct,
                RouteEvidenceKind.ObservedZoneEntry,
                wing,
                model.ElapsedSeconds,
                zoneLabel,
                hasObservedPosition: true,
                observedPosition));
        }

        public static bool TryRecordInferredStimulus(
            ThreatPressureModel model,
            ThreatWing wing,
            bool threatCanHear,
            ThreatStimulus stimulus,
            float heardStrength,
            float minimumStrength)
        {
            if (model == null || wing == ThreatWing.None || !threatCanHear ||
                heardStrength < Mathf.Clamp01(minimumStrength))
            {
                return false;
            }

            var kind = ToEvidenceKind(stimulus.Kind);
            if (kind == RouteEvidenceKind.None)
            {
                return false;
            }

            return model.RecordRouteEvidence(new RouteEvidence(
                RouteEvidenceSource.Inferred,
                kind,
                wing,
                model.ElapsedSeconds,
                $"{wing} route"));
        }

        private static RouteEvidenceKind ToEvidenceKind(ThreatStimulusKind kind)
        {
            return kind switch
            {
                ThreatStimulusKind.Sprint => RouteEvidenceKind.Sprint,
                ThreatStimulusKind.JumpLanding => RouteEvidenceKind.JumpLanding,
                ThreatStimulusKind.DroppedProp => RouteEvidenceKind.DroppedProp,
                ThreatStimulusKind.RodRejected => RouteEvidenceKind.RodRejected,
                ThreatStimulusKind.ObjectiveProgress => RouteEvidenceKind.ObjectiveProgress,
                _ => RouteEvidenceKind.None,
            };
        }
    }
}
