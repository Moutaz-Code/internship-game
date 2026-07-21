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
        public void RepeatedWingVisitsCreateTemporaryPatrolBias()
        {
            var model = new ThreatPressureModel();

            model.RecordWingVisit(ThreatWing.West);
            Assert.That(model.BiasedWing, Is.EqualTo(ThreatWing.None));

            model.RecordWingVisit(ThreatWing.West);
            Assert.That(model.BiasedWing, Is.EqualTo(ThreatWing.West));
            Assert.That(model.GetPatrolWeight(ThreatWing.West), Is.EqualTo(ThreatPressureModel.BiasedPatrolWeight));
            Assert.That(model.GetPatrolWeight(ThreatWing.East), Is.EqualTo(1f));

            model.Tick(ThreatPressureModel.PatrolBiasDurationSeconds);
            Assert.That(model.BiasedWing, Is.EqualTo(ThreatWing.None));
            Assert.That(model.GetPatrolWeight(ThreatWing.West), Is.EqualTo(1f));
        }

        [Test]
        public void EqualInputStreamsProduceIdenticalPressureAndBias()
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
                first.RecordWingVisit((ThreatWing)(1 + i % 3));
                second.RecordWingVisit((ThreatWing)(1 + i % 3));
                first.Tick(0.1f);
                second.Tick(0.1f);

                var a = first.Snapshot;
                var b = second.Snapshot;
                Assert.That(a.TotalPressure, Is.EqualTo(b.TotalPressure));
                Assert.That(a.NoisePressure, Is.EqualTo(b.NoisePressure));
                Assert.That(a.BiasedWing, Is.EqualTo(b.BiasedWing));
                Assert.That(a.BiasSecondsRemaining, Is.EqualTo(b.BiasSecondsRemaining));
            }
        }
    }
}
