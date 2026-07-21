using NUnit.Framework;

namespace InternshipGame.Prototype.Tests
{
    public sealed class PursuitThreatStateMachineTests
    {
        [Test]
        public void ActivationProvidesWarningBeforeHunt()
        {
            var machine = CreateMachine();

            Assert.That(machine.Activate(), Is.True);
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Investigating));

            machine.Tick(0.9f, true, 3f);
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Investigating));

            machine.Tick(0.2f, true, 3f);
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Hunting));
        }

        [Test]
        public void LosingSightTransitionsFromHuntToSearch()
        {
            var machine = CreateMachine();
            machine.Activate();
            machine.Tick(1.1f, true, 3f);

            machine.Tick(0.4f, false, 3f);
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Hunting));

            machine.Tick(0.7f, false, 3f);
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Searching));
        }

        [Test]
        public void SearchCanReacquirePlayer()
        {
            var machine = CreateMachine();
            machine.Activate();
            machine.Tick(1.1f, false, 3f);
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Searching));

            machine.Tick(0.1f, true, 3f);
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Hunting));
        }

        [Test]
        public void SearchTimeoutDisengagesToPatrol()
        {
            var machine = CreateMachine();
            machine.Activate();
            machine.Tick(1.1f, false, 3f);
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Searching));

            machine.Tick(3f, false, 3f);
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Patrolling));
            Assert.That(machine.Investigate(), Is.True);
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Investigating));
        }

        [Test]
        public void AdaptedSearchMultiplierExtendsSearchWithoutChangingBaselineTiming()
        {
            var baseline = CreateMachine();
            var adapted = CreateMachine();
            baseline.Activate();
            adapted.Activate();
            baseline.Tick(1.1f, false, 3f);
            adapted.Tick(1.1f, false, 3f);

            baseline.Tick(3f, false, 3f);
            adapted.Tick(3f, false, 3f, 1.35f);

            Assert.That(baseline.State, Is.EqualTo(PursuitThreatState.Patrolling));
            Assert.That(adapted.State, Is.EqualTo(PursuitThreatState.Searching));
            adapted.Tick(1.1f, false, 3f, 1.35f);
            Assert.That(adapted.State, Is.EqualTo(PursuitThreatState.Patrolling));
        }

        [Test]
        public void CaptureOnlyOccursAfterWarningPhase()
        {
            var machine = CreateMachine();
            machine.Activate();
            machine.Tick(0.5f, true, 0.2f);
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Investigating));

            machine.Tick(0.6f, true, 0.2f);
            machine.Tick(0.01f, true, 0.2f);
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Caught));
        }

        [Test]
        public void OcclusionPreventsCloseRangeCaptureUntilSightIsReacquired()
        {
            var machine = CreateMachine();
            machine.Activate();
            machine.Tick(1.1f, true, 3f);
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Hunting));

            machine.Tick(0.1f, false, 0.2f);
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Hunting));

            machine.Tick(1f, false, 0.2f);
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Searching));

            machine.Tick(0.1f, false, 0.2f);
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Searching));

            machine.Tick(0.1f, true, 0.2f);
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Caught));
        }

        [Test]
        public void ResetMakesThreatReplayable()
        {
            var machine = CreateMachine();
            machine.Activate();
            machine.Tick(1.1f, true, 0.2f);
            machine.Tick(0.01f, true, 0.2f);

            machine.Reset();
            Assert.That(machine.State, Is.EqualTo(PursuitThreatState.Dormant));
            Assert.That(machine.Activate(), Is.True);
        }

        private static PursuitThreatStateMachine CreateMachine()
        {
            return new PursuitThreatStateMachine(1f, 1f, 3f, 0.8f);
        }
    }
}
