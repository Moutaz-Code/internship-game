using NUnit.Framework;

namespace InternshipGame.Prototype.Tests
{
    public sealed class ArchitectureSeamsTests
    {
        [TestCase(PlayerControlMode.Desktop, false, PlayerControlMode.Desktop)]
        [TestCase(PlayerControlMode.Desktop, true, PlayerControlMode.Desktop)]
        [TestCase(PlayerControlMode.VR, false, PlayerControlMode.VR)]
        [TestCase(PlayerControlMode.Auto, false, PlayerControlMode.Desktop)]
        [TestCase(PlayerControlMode.Auto, true, PlayerControlMode.VR)]
        public void PlayerModeResolution_IsExplicitAndDeterministic(
            PlayerControlMode configured,
            bool xrAvailable,
            PlayerControlMode expected)
        {
            Assert.That(PlayerControlModeResolver.Resolve(configured, xrAvailable), Is.EqualTo(expected));
        }

        [Test]
        public void DeviceFaultGate_IsolatesAndThenAllowsRecovery()
        {
            var gate = new DeviceFaultGate(3, 5f);

            gate.MarkFailure(10f);
            gate.MarkFailure(10f);
            Assert.That(gate.IsFaulted, Is.False);

            gate.MarkFailure(10f);
            Assert.That(gate.IsFaulted, Is.True);
            Assert.That(gate.CanAttempt(14.99f), Is.False);
            Assert.That(gate.CanAttempt(15f), Is.True);

            gate.MarkSuccess();
            Assert.That(gate.IsFaulted, Is.False);
            Assert.That(gate.ConsecutiveFailures, Is.Zero);
        }

        [Test]
        public void FeedbackSignal_ClampsUnsafeDeviceValues()
        {
            var signal = new FeedbackSignal(
                "test",
                FeedbackCommand.Pulse,
                FeedbackChannel.Heat,
                4f,
                -2f);

            Assert.That(signal.Intensity, Is.EqualTo(1f));
            Assert.That(signal.DurationSeconds, Is.Zero);
        }

        [Test]
        public void RunObjectiveContract_ReportsAggregateProgressAndCompletion()
        {
            Assert.That(typeof(IRunObjective).GetProperty(nameof(IRunObjective.Progress)), Is.Not.Null);
            Assert.That(typeof(IRunObjective).GetProperty(nameof(IRunObjective.IsComplete)), Is.Not.Null);
        }
    }
}
