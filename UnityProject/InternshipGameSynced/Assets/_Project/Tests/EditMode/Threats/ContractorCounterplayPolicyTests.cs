using NUnit.Framework;

namespace InternshipGame.Prototype.Tests
{
    public sealed class ContractorCounterplayPolicyTests
    {
        [Test]
        public void ThrowableImpact_AcceptsExactlyOneImpactPerRelease()
        {
            var gate = new ThrowableImpactGate();
            gate.OnReleased();

            Assert.That(gate.TryAccept(1f, 2f, 1.2f, 0.7f), Is.True);
            Assert.That(gate.TryAccept(2f, 6f, 1.2f, 0.7f), Is.False);
            Assert.That(gate.AcceptedImpactCount, Is.EqualTo(1));
        }

        [Test]
        public void ThrowableImpact_RejectsBounceAndCooldownRetrigger()
        {
            var gate = new ThrowableImpactGate();
            gate.OnReleased();
            Assert.That(gate.TryAccept(1f, 1f, 1.2f, 0.7f), Is.False);
            Assert.That(gate.IsArmed, Is.True);
            Assert.That(gate.TryAccept(1.1f, 3f, 1.2f, 0.7f), Is.True);

            gate.OnReleased();
            Assert.That(gate.TryAccept(1.5f, 3f, 1.2f, 0.7f), Is.False);
            Assert.That(gate.TryAccept(1.8f, 3f, 1.2f, 0.7f), Is.True);
        }

        [Test]
        public void ThrowableImpact_ResetRestoresUnarmedBaseline()
        {
            var gate = new ThrowableImpactGate();
            gate.OnReleased();
            Assert.That(gate.TryAccept(1f, 3f, 1.2f, 0.7f), Is.True);

            gate.Reset();

            Assert.That(gate.IsArmed, Is.False);
            Assert.That(gate.AcceptedImpactCount, Is.Zero);
            Assert.That(gate.NextAllowedTime, Is.Zero);
        }

        [Test]
        public void MachineryPanel_UsesLockedActiveAndCooldownWindows()
        {
            var state = new MachineryPanelState(2.5f, 25f);

            Assert.That(state.TryActivate(10f), Is.True);
            Assert.That(state.IsActive(12.49f), Is.True);
            Assert.That(state.IsActive(12.5f), Is.False);
            Assert.That(state.IsReady(34.99f), Is.False);
            Assert.That(state.IsReady(35f), Is.True);
        }

        [Test]
        public void MachineryPanel_CooldownRejectsRepeatAndResetRestoresReady()
        {
            var state = new MachineryPanelState(2.5f, 25f);
            Assert.That(state.TryActivate(0f), Is.True);
            Assert.That(state.TryActivate(3f), Is.False);

            state.Reset();

            Assert.That(state.IsReady(0f), Is.True);
            Assert.That(state.ActivationCount, Is.Zero);
        }

        [Test]
        public void MachineryStimulus_PressureModelRecognizesNewKind()
        {
            var model = new ThreatPressureModel();
            model.SetThreatPosition(UnityEngine.Vector3.zero);

            model.ReportStimulus(new ThreatStimulus(
                ThreatStimulusKind.Machinery,
                UnityEngine.Vector3.zero,
                0.75f,
                18f,
                0f));

            Assert.That(model.LastStimulusStrength, Is.EqualTo(0.75f).Within(0.0001f));
            Assert.That(model.NoisePressure, Is.GreaterThan(0f));
        }
    }
}
