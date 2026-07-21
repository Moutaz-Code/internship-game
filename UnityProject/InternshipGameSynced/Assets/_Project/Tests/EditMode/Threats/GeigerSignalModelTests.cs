using System.Collections.Generic;
using NUnit.Framework;

namespace InternshipGame.Prototype.Tests
{
    public sealed class GeigerSignalModelTests
    {
        [Test]
        public void DisabledInterferenceAlwaysReturnsTruthSignal()
        {
            var model = new GeigerSignalModel(17, interferenceEnabled: false);

            for (var i = 0; i < 200; i++)
            {
                var reading = model.Tick(0.1f, 10f, owlActive: true, owlDistance: 1f);

                Assert.That(reading.InterferenceActive, Is.False);
                Assert.That(reading.FalseBurstActive, Is.False);
                Assert.That(reading.JitterMultiplier, Is.EqualTo(1f));
                Assert.That(reading.ReportedClicksPerSecond, Is.EqualTo(reading.TruthClicksPerSecond));
            }
        }

        [Test]
        public void OwlInterferenceIsRangeBoundAndJitterNeverExceedsTwentyPercent()
        {
            var model = new GeigerSignalModel(29);

            for (var i = 0; i < 100; i++)
            {
                var reading = model.Tick(0.1f, 12f, owlActive: true, owlDistance: 8f);
                Assert.That(reading.InterferenceActive, Is.True);
                Assert.That(reading.JitterMultiplier, Is.InRange(0.8f, 1.2f));
            }

            var outOfRange = model.Tick(0.1f, 12f, owlActive: true, owlDistance: 8.01f);
            Assert.That(outOfRange.InterferenceActive, Is.False);
            Assert.That(outOfRange.FalseBurstActive, Is.False);
            Assert.That(outOfRange.ReportedClicksPerSecond, Is.EqualTo(outOfRange.TruthClicksPerSecond));

            var inactive = model.Tick(0.1f, 12f, owlActive: false, owlDistance: 1f);
            Assert.That(inactive.InterferenceActive, Is.False);
        }

        [Test]
        public void FalseBurstsAreAtMostOneSecondAndAtLeastTwelveSecondsApart()
        {
            const float step = 0.1f;
            var model = new GeigerSignalModel(43);
            var burstStarts = new List<float>();
            var burstDuration = 0f;
            var previousBurst = false;

            for (var i = 0; i < 700; i++)
            {
                var reading = model.Tick(step, float.PositiveInfinity, owlActive: true, owlDistance: 2f);
                if (reading.FalseBurstActive && !previousBurst)
                {
                    burstStarts.Add(model.ElapsedSeconds);
                    burstDuration = 0f;
                }

                if (reading.FalseBurstActive)
                {
                    burstDuration += step;
                    Assert.That(burstDuration, Is.LessThanOrEqualTo(1.01f));
                }

                previousBurst = reading.FalseBurstActive;
            }

            Assert.That(burstStarts.Count, Is.GreaterThanOrEqualTo(4));
            for (var i = 1; i < burstStarts.Count; i++)
            {
                Assert.That(burstStarts[i] - burstStarts[i - 1], Is.GreaterThanOrEqualTo(11.99f));
            }
        }

        [Test]
        public void EqualSeedsAndInputsProduceIdenticalReadings()
        {
            var first = new GeigerSignalModel(991);
            var second = new GeigerSignalModel(991);

            for (var i = 0; i < 400; i++)
            {
                var distance = 24f - i % 20;
                var owlActive = i % 17 != 0;
                var owlDistance = i % 11;
                var a = first.Tick(0.05f, distance, owlActive, owlDistance);
                var b = second.Tick(0.05f, distance, owlActive, owlDistance);

                Assert.That(a.TruthClicksPerSecond, Is.EqualTo(b.TruthClicksPerSecond));
                Assert.That(a.ReportedClicksPerSecond, Is.EqualTo(b.ReportedClicksPerSecond));
                Assert.That(a.JitterMultiplier, Is.EqualTo(b.JitterMultiplier));
                Assert.That(a.InterferenceActive, Is.EqualTo(b.InterferenceActive));
                Assert.That(a.FalseBurstActive, Is.EqualTo(b.FalseBurstActive));
            }
        }
    }
}
