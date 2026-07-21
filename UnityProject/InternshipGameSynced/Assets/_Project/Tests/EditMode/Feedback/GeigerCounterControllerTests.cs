using NUnit.Framework;
using UnityEngine;

namespace InternshipGame.Prototype.Tests
{
    public sealed class GeigerCounterControllerTests
    {
        [TestCase(PrototypeRunState.Extracted)]
        [TestCase(PrototypeRunState.Failed)]
        public void TerminalRunStateSuppressesGeigerUntilReset(PrototypeRunState terminalState)
        {
            var gameObject = new GameObject("Geiger Test");
            try
            {
                var geiger = gameObject.AddComponent<GeigerCounterController>();

                geiger.OnRunStateChanged(PrototypeRunState.RetrieveFuelRods, terminalState);
                Assert.That(geiger.IsOutputSuppressed, Is.True);
                Assert.That(geiger.Reading.ReportedClicksPerSecond, Is.Zero);

                geiger.OnRunReset();
                Assert.That(geiger.IsOutputSuppressed, Is.False);
            }
            finally
            {
                Object.DestroyImmediate(gameObject);
            }
        }
    }
}
