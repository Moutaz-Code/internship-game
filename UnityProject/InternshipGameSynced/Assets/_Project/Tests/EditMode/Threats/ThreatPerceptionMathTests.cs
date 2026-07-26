using NUnit.Framework;
using UnityEngine;

namespace InternshipGame.Prototype.Tests
{
    public sealed class ThreatPerceptionMathTests
    {
        [Test]
        public void ViewCone_UsesWiderButStillDirectionalNearAwareness()
        {
            var eye = Vector3.up;
            var behindAndNear = eye + Quaternion.Euler(0f, 100f, 0f) * Vector3.forward * 2f;
            var sideAndNear = eye + Quaternion.Euler(0f, 75f, 0f) * Vector3.forward * 2f;

            Assert.That(
                ThreatPerceptionMath.IsWithinViewCone(
                    eye, Vector3.forward, sideAndNear, 22f, 120f, 2.5f, 170f),
                Is.True);
            Assert.That(
                ThreatPerceptionMath.IsWithinViewCone(
                    eye, Vector3.forward, behindAndNear, 22f, 120f, 2.5f, 170f),
                Is.False,
                "Near awareness must not become omnidirectional knowledge.");
        }

        [Test]
        public void ViewCone_RejectsTargetsBeyondConfiguredRange()
        {
            Assert.That(
                ThreatPerceptionMath.IsWithinViewCone(
                    Vector3.zero, Vector3.forward, Vector3.forward * 22.1f, 22f, 120f, 2.5f, 170f),
                Is.False);
        }

        [Test]
        public void ObservationLead_IsPlanarAndBounded()
        {
            var predicted = ThreatPerceptionMath.PredictObservedPosition(
                new Vector3(2f, 1.6f, 3f),
                new Vector3(10f, 8f, 0f),
                0.35f,
                1.4f);

            Assert.That(predicted.y, Is.EqualTo(1.6f).Within(0.0001f));
            Assert.That(Vector3.Distance(new Vector3(2f, 1.6f, 3f), predicted), Is.EqualTo(1.4f).Within(0.0001f));
        }

        [Test]
        public void SearchPattern_SweepsFourDistinctDirections()
        {
            var offsets = new[]
            {
                ThreatPerceptionMath.SearchOffset(0, 3.25f),
                ThreatPerceptionMath.SearchOffset(1, 3.25f),
                ThreatPerceptionMath.SearchOffset(2, 3.25f),
                ThreatPerceptionMath.SearchOffset(3, 3.25f),
            };

            Assert.That(offsets, Has.All.Matches<Vector3>(offset => Mathf.Approximately(offset.magnitude, 3.25f)));
            Assert.That(offsets[0], Is.Not.EqualTo(offsets[1]));
            Assert.That(offsets[0] + offsets[2], Is.EqualTo(Vector3.zero));
            Assert.That(offsets[1] + offsets[3], Is.EqualTo(Vector3.zero));
        }
    }
}
