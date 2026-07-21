using NUnit.Framework;

namespace InternshipGame.Prototype.Tests
{
    public sealed class WatcherEncounterSequenceTests
    {
        [Test]
        public void ArmStartsRevealDelay()
        {
            var encounter = CreateSequence();

            var armed = encounter.Arm();

            Assert.That(armed, Is.True);
            Assert.That(encounter.State, Is.EqualTo(WatcherEncounterState.Waiting));
        }

        [Test]
        public void WatcherRevealsOnlyAfterDelay()
        {
            var encounter = CreateSequence();
            encounter.Arm();

            encounter.Tick(1f, viewerDistance: 8f);
            Assert.That(encounter.State, Is.EqualTo(WatcherEncounterState.Waiting));

            encounter.Tick(0.6f, viewerDistance: 8f);
            Assert.That(encounter.State, Is.EqualTo(WatcherEncounterState.Visible));
        }

        [Test]
        public void ApproachingVisibleWatcherCompletesEncounter()
        {
            var encounter = CreateSequence();
            encounter.Arm();
            encounter.Tick(2f, viewerDistance: 8f);

            encounter.Tick(0.1f, viewerDistance: 2f);

            Assert.That(encounter.State, Is.EqualTo(WatcherEncounterState.Complete));
        }

        [Test]
        public void VisibleWatcherLeavesAfterDuration()
        {
            var encounter = CreateSequence();
            encounter.Arm();
            encounter.Tick(2f, viewerDistance: 8f);

            encounter.Tick(8f, viewerDistance: 8f);

            Assert.That(encounter.State, Is.EqualTo(WatcherEncounterState.Complete));
        }

        [Test]
        public void ResetMakesEncounterReplayable()
        {
            var encounter = CreateSequence();
            encounter.Arm();
            encounter.Dismiss();

            encounter.Reset();

            Assert.That(encounter.State, Is.EqualTo(WatcherEncounterState.Dormant));
            Assert.That(encounter.Arm(), Is.True);
        }

        private static WatcherEncounterSequence CreateSequence()
        {
            return new WatcherEncounterSequence(
                revealDelaySeconds: 1.5f,
                visibleDurationSeconds: 8f,
                dismissDistance: 2.2f);
        }
    }
}
