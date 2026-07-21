using NUnit.Framework;
using UnityEngine;

namespace InternshipGame.Prototype.Tests
{
    public sealed class ThreatPressureModelTests
    {
        [Test]
        public void PressureComponentsStayNormalizedAndNoiseDecays()
        {
            var model = new ThreatPressureModel();
            model.SetThreatPosition(Vector3.zero);
            model.SetObjectiveProgress(2f);
            model.ReportStimulus(new ThreatStimulus(
                ThreatStimulusKind.RodRejected,
                Vector3.zero,
                1f,
                12f));
            model.Tick(ThreatPressureModel.TimeToMaximumPressureSeconds);

            var peak = model.Snapshot;
            Assert.That(peak.TotalPressure, Is.InRange(0f, 1f));
            Assert.That(peak.TimePressure, Is.InRange(0f, 1f));
            Assert.That(peak.ObjectivePressure, Is.InRange(0f, 1f));
            Assert.That(peak.NoisePressure, Is.InRange(0f, 1f));

            model.SetObjectiveProgress(0f);
            model.Tick(20f);
            Assert.That(model.NoisePressure, Is.EqualTo(0f));
        }

        [Test]
        public void PositionalStimuliUseFalloffAndThenDecay()
        {
            var nearby = new ThreatPressureModel();
            nearby.SetThreatPosition(Vector3.zero);
            nearby.ReportStimulus(new ThreatStimulus(
                ThreatStimulusKind.DroppedProp,
                new Vector3(2f, 0f, 0f),
                0.8f,
                10f));

            var distant = new ThreatPressureModel();
            distant.SetThreatPosition(new Vector3(20f, 0f, 0f));
            distant.ReportStimulus(new ThreatStimulus(
                ThreatStimulusKind.DroppedProp,
                new Vector3(2f, 0f, 0f),
                0.8f,
                10f));

            Assert.That(nearby.NoisePressure, Is.GreaterThan(distant.NoisePressure));
            Assert.That(distant.NoisePressure, Is.EqualTo(0f));

            var beforeDecay = nearby.NoisePressure;
            nearby.Tick(1f);
            Assert.That(nearby.NoisePressure, Is.EqualTo(beforeDecay - ThreatPressureModel.NoiseDecayPerSecond).Within(0.0001f));
        }

        [Test]
        public void UnperceivedRouteEntryCreatesNoEvidence()
        {
            var model = new ThreatPressureModel();

            var silentEntry = ThreatRouteEvidencePolicy.TryRecordDirectObservation(
                model,
                ThreatWing.West,
                threatIsActive: true,
                hasLineOfSight: false,
                "WestMaintenance",
                new Vector3(-10f, 0f, 2f));
            var dormantObservation = ThreatRouteEvidencePolicy.TryRecordDirectObservation(
                model,
                ThreatWing.West,
                threatIsActive: false,
                hasLineOfSight: true,
                "WestMaintenance",
                new Vector3(-10f, 0f, 2f));

            Assert.That(silentEntry, Is.False);
            Assert.That(dormantObservation, Is.False);
            Assert.That(model.GetRouteConfidence(ThreatWing.West), Is.Zero);
            Assert.That(model.LastRouteEvidence, Is.Null);
            Assert.That(model.Adaptation.State, Is.EqualTo(RouteAdaptationState.Uncertain));
        }

        [Test]
        public void DirectEvidenceHasTwiceTheDefaultInferredWeight()
        {
            var directModel = new ThreatPressureModel();
            var inferredModel = new ThreatPressureModel();

            Assert.That(directModel.RecordRouteEvidence(Direct(directModel, ThreatWing.West)), Is.True);
            Assert.That(inferredModel.RecordRouteEvidence(Inferred(inferredModel, ThreatWing.West)), Is.True);

            Assert.That(directModel.GetRouteConfidence(ThreatWing.West), Is.EqualTo(1f));
            Assert.That(inferredModel.GetRouteConfidence(ThreatWing.West), Is.EqualTo(0.5f));
            Assert.That(directModel.LastRouteEvidence.Value.HasObservedPosition, Is.True);
            Assert.That(inferredModel.LastRouteEvidence.Value.HasObservedPosition, Is.False);
        }

        [Test]
        public void InferredEvidenceIsRateLimitedAndCanCorroborateToSuspicious()
        {
            var model = new ThreatPressureModel();

            Assert.That(model.RecordRouteEvidence(Inferred(model, ThreatWing.Central)), Is.True);
            Assert.That(model.RecordRouteEvidence(Inferred(model, ThreatWing.Central)), Is.False);
            Assert.That(model.GetRouteConfidence(ThreatWing.Central), Is.EqualTo(0.5f));
            Assert.That(model.Adaptation.State, Is.EqualTo(RouteAdaptationState.Uncertain));

            model.Tick(model.RouteTuning.InferredEvidenceCooldownSeconds);
            Assert.That(model.RecordRouteEvidence(Inferred(model, ThreatWing.Central)), Is.True);
            Assert.That(model.GetRouteConfidence(ThreatWing.Central), Is.EqualTo(1f));
            Assert.That(model.Adaptation.State, Is.EqualTo(RouteAdaptationState.Suspicious));
        }

        [Test]
        public void OnlySufficientlyStrongRouteLocalStimulusCreatesInferredEvidence()
        {
            var model = new ThreatPressureModel();
            var stimulus = new ThreatStimulus(
                ThreatStimulusKind.JumpLanding,
                new Vector3(11f, 0f, 2f),
                0.7f,
                15f);

            Assert.That(
                ThreatRouteEvidencePolicy.TryRecordInferredStimulus(
                    model,
                    ThreatWing.East,
                    threatCanHear: true,
                    stimulus,
                    heardStrength: 0.29f,
                    minimumStrength: 0.3f),
                Is.False);
            Assert.That(
                ThreatRouteEvidencePolicy.TryRecordInferredStimulus(
                    model,
                    ThreatWing.East,
                    threatCanHear: true,
                    stimulus,
                    heardStrength: 0.3f,
                    minimumStrength: 0.3f),
                Is.True);
            Assert.That(model.GetRouteConfidence(ThreatWing.East), Is.EqualTo(0.5f));
            Assert.That(model.LastRouteEvidence.Value.Source, Is.EqualTo(RouteEvidenceSource.Inferred));
            Assert.That(model.LastRouteEvidence.Value.Kind, Is.EqualTo(RouteEvidenceKind.JumpLanding));
            Assert.That(model.LastRouteEvidence.Value.HasObservedPosition, Is.False);
        }

        [Test]
        public void DirectObservationsCrossSuspiciousThenAdaptedThresholds()
        {
            var model = new ThreatPressureModel();

            model.RecordRouteEvidence(Direct(model, ThreatWing.West));
            Assert.That(model.Adaptation.State, Is.EqualTo(RouteAdaptationState.Suspicious));
            Assert.That(model.BiasedWing, Is.EqualTo(ThreatWing.None));
            Assert.That(model.GetPatrolWeight(ThreatWing.West), Is.EqualTo(1.25f));

            model.RecordRouteEvidence(Direct(model, ThreatWing.West));
            Assert.That(model.Adaptation.State, Is.EqualTo(RouteAdaptationState.Adapted));
            Assert.That(model.BiasedWing, Is.EqualTo(ThreatWing.West));
            Assert.That(model.GetPatrolWeight(ThreatWing.West), Is.EqualTo(ThreatPressureModel.BiasedPatrolWeight));
            Assert.That(model.GetPatrolWeight(ThreatWing.East), Is.EqualTo(1f));
            Assert.That(model.Adaptation.BiasSecondsRemaining, Is.EqualTo(60f));
        }

        [Test]
        public void RouteVarietyWeakensOldPredictionAndCreatesCounterAdaptedOpening()
        {
            var model = new ThreatPressureModel();
            model.RecordRouteEvidence(Direct(model, ThreatWing.West));
            model.RecordRouteEvidence(Direct(model, ThreatWing.West));
            var oldConfidence = model.GetRouteConfidence(ThreatWing.West);

            model.RecordRouteEvidence(Direct(model, ThreatWing.East));

            Assert.That(model.GetRouteConfidence(ThreatWing.West), Is.LessThan(oldConfidence));
            Assert.That(model.GetRouteConfidence(ThreatWing.West), Is.EqualTo(1f));
            Assert.That(model.GetRouteConfidence(ThreatWing.East), Is.EqualTo(1f));
            Assert.That(model.Adaptation.State, Is.EqualTo(RouteAdaptationState.CounterAdapted));
            Assert.That(model.Adaptation.DominantWing, Is.EqualTo(ThreatWing.East));
            Assert.That(model.PreferredPatrolWing, Is.EqualTo(ThreatWing.None));
            Assert.That(model.GetPatrolWeight(ThreatWing.West), Is.EqualTo(1f));

            model.Tick(model.RouteTuning.CounterAdaptedDurationSeconds);
            Assert.That(model.Adaptation.State, Is.EqualTo(RouteAdaptationState.Suspicious));
            Assert.That(model.PreferredPatrolWing, Is.EqualTo(ThreatWing.East));
        }

        [Test]
        public void ConfidenceDecayChangesStateAfterConfiguredDelay()
        {
            var model = new ThreatPressureModel();
            model.RecordRouteEvidence(Direct(model, ThreatWing.Central));

            model.Tick(model.RouteTuning.ConfidenceDecayDelaySeconds - 1f);
            Assert.That(model.Adaptation.State, Is.EqualTo(RouteAdaptationState.Suspicious));
            Assert.That(model.GetRouteConfidence(ThreatWing.Central), Is.EqualTo(1f));

            model.Tick(2f);
            Assert.That(model.GetRouteConfidence(ThreatWing.Central), Is.LessThan(1f));
            Assert.That(model.Adaptation.State, Is.EqualTo(RouteAdaptationState.Uncertain));
        }

        [Test]
        public void ResetClearsAllRunLocalRouteMemory()
        {
            var model = new ThreatPressureModel();
            model.RecordRouteEvidence(Direct(model, ThreatWing.West));
            model.RecordRouteEvidence(Direct(model, ThreatWing.West));
            model.RecordPatrolSelection(ThreatWing.West, RouteAdaptationResponse.AdaptedRouteBias);
            var resetBefore = model.ResetSequence;

            model.Reset();

            Assert.That(model.Adaptation.State, Is.EqualTo(RouteAdaptationState.Uncertain));
            Assert.That(model.Adaptation.DominantWing, Is.EqualTo(ThreatWing.None));
            Assert.That(model.GetRouteConfidence(ThreatWing.West), Is.Zero);
            Assert.That(model.GetRouteConfidence(ThreatWing.Central), Is.Zero);
            Assert.That(model.GetRouteConfidence(ThreatWing.East), Is.Zero);
            Assert.That(model.LastRouteEvidence, Is.Null);
            Assert.That(model.ConsecutiveBiasedPatrolSelections, Is.Zero);
            Assert.That(model.Adaptation.BiasSecondsRemaining, Is.Zero);
            Assert.That(model.ResetSequence, Is.EqualTo(resetBefore + 1));
            Assert.That(model.LastResetReason, Is.EqualTo("run reset"));
        }

        [Test]
        public void PatrolBiasIsCappedAndThirdConsecutiveChoiceMustVary()
        {
            var tuning = new RouteAdaptationTuning(
                1f,
                0.5f,
                1f,
                2f,
                4f,
                60f,
                0.025f,
                1f,
                1.25f,
                5f,
                60f,
                12f,
                2);
            var model = new ThreatPressureModel(tuning);
            model.RecordRouteEvidence(Direct(model, ThreatWing.West));
            model.RecordRouteEvidence(Direct(model, ThreatWing.West));
            Assert.That(model.GetPatrolWeight(ThreatWing.West), Is.EqualTo(ThreatPressureModel.BiasedPatrolWeight));

            var patrolWings = new[]
            {
                ThreatWing.West,
                ThreatWing.Central,
                ThreatWing.West,
                ThreatWing.East,
            };
            var index = 3;
            for (var choice = 0; choice < 2; choice++)
            {
                var selection = ThreatPatrolSelectionPolicy.SelectNext(
                    patrolWings,
                    index,
                    model.PreferredPatrolWing,
                    model.RequiresPatrolVariety);
                Assert.That(selection.Wing, Is.EqualTo(ThreatWing.West));
                model.RecordPatrolSelection(selection.Wing, RouteAdaptationResponse.AdaptedRouteBias);
                index = selection.Index;
            }

            Assert.That(model.RequiresPatrolVariety, Is.True);
            var variedSelection = ThreatPatrolSelectionPolicy.SelectNext(
                patrolWings,
                index,
                model.PreferredPatrolWing,
                model.RequiresPatrolVariety);
            Assert.That(variedSelection.ForcedVariety, Is.True);
            Assert.That(variedSelection.Wing, Is.Not.EqualTo(ThreatWing.West));
        }

        [Test]
        public void AdaptationDisabledClearsMemoryAndRestoresBaselineWeights()
        {
            var model = new ThreatPressureModel();
            model.RecordRouteEvidence(Direct(model, ThreatWing.East));
            model.RecordRouteEvidence(Direct(model, ThreatWing.East));

            model.SetAdaptationEnabled(false);

            Assert.That(model.Adaptation.Enabled, Is.False);
            Assert.That(model.Adaptation.State, Is.EqualTo(RouteAdaptationState.Uncertain));
            Assert.That(model.GetRouteConfidence(ThreatWing.East), Is.Zero);
            Assert.That(model.GetPatrolWeight(ThreatWing.East), Is.EqualTo(1f));
            Assert.That(model.RecordRouteEvidence(Direct(model, ThreatWing.East)), Is.False);

            model.SetAdaptationEnabled(true);
            Assert.That(model.Adaptation.Enabled, Is.True);
            Assert.That(model.GetRouteConfidence(ThreatWing.East), Is.Zero);
            Assert.That(model.GetPatrolWeight(ThreatWing.East), Is.EqualTo(1f));
        }

        [Test]
        public void EqualInputStreamsProduceIdenticalPressureAndAdaptation()
        {
            var first = new ThreatPressureModel();
            var second = new ThreatPressureModel();

            for (var i = 0; i < 80; i++)
            {
                var position = new Vector3(i % 4, 0f, i % 7);
                var stimulus = new ThreatStimulus(
                    (ThreatStimulusKind)(i % 6),
                    position,
                    (i % 10) / 10f,
                    12f,
                    i * 0.1f);

                first.SetThreatPosition(Vector3.zero);
                second.SetThreatPosition(Vector3.zero);
                first.SetObjectiveProgress(i / 80f);
                second.SetObjectiveProgress(i / 80f);
                first.ReportStimulus(stimulus);
                second.ReportStimulus(stimulus);
                var wing = (ThreatWing)(1 + i % 3);
                var firstEvidence = i % 2 == 0 ? Direct(first, wing) : Inferred(first, wing);
                var secondEvidence = i % 2 == 0 ? Direct(second, wing) : Inferred(second, wing);
                Assert.That(first.RecordRouteEvidence(firstEvidence), Is.EqualTo(second.RecordRouteEvidence(secondEvidence)));
                first.Tick(0.1f);
                second.Tick(0.1f);

                var a = first.Snapshot;
                var b = second.Snapshot;
                var routeA = first.Adaptation;
                var routeB = second.Adaptation;
                Assert.That(a.TotalPressure, Is.EqualTo(b.TotalPressure));
                Assert.That(a.NoisePressure, Is.EqualTo(b.NoisePressure));
                Assert.That(routeA.State, Is.EqualTo(routeB.State));
                Assert.That(routeA.WestConfidence, Is.EqualTo(routeB.WestConfidence));
                Assert.That(routeA.CentralConfidence, Is.EqualTo(routeB.CentralConfidence));
                Assert.That(routeA.EastConfidence, Is.EqualTo(routeB.EastConfidence));
                Assert.That(routeA.DominantWing, Is.EqualTo(routeB.DominantWing));
            }
        }

        private static RouteEvidence Direct(ThreatPressureModel model, ThreatWing wing)
        {
            return new RouteEvidence(
                RouteEvidenceSource.Direct,
                RouteEvidenceKind.ObservedZoneEntry,
                wing,
                model.ElapsedSeconds,
                $"{wing}Zone",
                hasObservedPosition: true,
                observedPosition: new Vector3((int)wing * 5f, 0f, 1f));
        }

        private static RouteEvidence Inferred(ThreatPressureModel model, ThreatWing wing)
        {
            return new RouteEvidence(
                RouteEvidenceSource.Inferred,
                RouteEvidenceKind.DroppedProp,
                wing,
                model.ElapsedSeconds,
                $"{wing} route");
        }
    }
}
