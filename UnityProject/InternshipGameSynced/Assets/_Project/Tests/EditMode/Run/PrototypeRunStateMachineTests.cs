using NUnit.Framework;

namespace InternshipGame.Prototype.Tests
{
    public sealed class PrototypeRunStateMachineTests
    {
        [Test]
        public void StartRunRequiresFuelRod()
        {
            var run = new PrototypeRunStateMachine();

            run.StartRun();

            Assert.That(run.State, Is.EqualTo(PrototypeRunState.FindFuelRod));
        }

        [Test]
        public void CollectingFuelRodChangesObjectiveToExtraction()
        {
            var run = new PrototypeRunStateMachine();
            run.StartRun();

            var changed = run.CollectFuelRod();

            Assert.That(changed, Is.True);
            Assert.That(run.State, Is.EqualTo(PrototypeRunState.ReturnToExtraction));
        }

        [Test]
        public void CannotExtractBeforeCollectingFuelRod()
        {
            var run = new PrototypeRunStateMachine();
            run.StartRun();

            var extracted = run.Extract(hasFuelRod: false);

            Assert.That(extracted, Is.False);
            Assert.That(run.State, Is.EqualTo(PrototypeRunState.FindFuelRod));
        }

        [Test]
        public void FuelRodCompletesExtractionLoop()
        {
            var run = new PrototypeRunStateMachine();
            run.StartRun();
            run.CollectFuelRod();

            var extracted = run.Extract(hasFuelRod: true);

            Assert.That(extracted, Is.True);
            Assert.That(run.State, Is.EqualTo(PrototypeRunState.Extracted));
        }

        [Test]
        public void TimeoutFailsAnActiveRun()
        {
            var run = new PrototypeRunStateMachine();
            run.StartRun();

            var failed = run.Fail();

            Assert.That(failed, Is.True);
            Assert.That(run.State, Is.EqualTo(PrototypeRunState.Failed));
        }

        [Test]
        public void MultiRodRunCyclesForTwentySecondsBeforeExitOpens()
        {
            var run = new PrototypeRunStateMachine();
            run.StartMultiRodRun();

            Assert.That(run.BeginExtractionCycle(), Is.True);
            Assert.That(run.State, Is.EqualTo(PrototypeRunState.ExtractionCycling));
            Assert.That(run.AdvanceExtractionCycle(19f), Is.False);
            Assert.That(run.ExtractionCycleRemaining, Is.EqualTo(1f).Within(0.001f));
            Assert.That(run.AdvanceExtractionCycle(1f), Is.True);
            Assert.That(run.State, Is.EqualTo(PrototypeRunState.EscapeReady));
        }

        [Test]
        public void MultiRodRunRequiresExitCrossingAfterCycle()
        {
            var run = new PrototypeRunStateMachine();
            run.StartMultiRodRun();

            Assert.That(run.Escape(), Is.False);
            run.BeginExtractionCycle();
            run.AdvanceExtractionCycle(20f);

            Assert.That(run.Escape(), Is.True);
            Assert.That(run.State, Is.EqualTo(PrototypeRunState.Extracted));
        }
    }
}
