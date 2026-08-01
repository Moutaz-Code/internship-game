using NUnit.Framework;
using UnityEngine;

namespace InternshipGame.Prototype.Tests
{
    public sealed class ReadableStealthPolicyTests
    {
        [TestCase(0, VisualExposureState.Concealed)]
        [TestCase(1, VisualExposureState.Noticeable)]
        [TestCase(2, VisualExposureState.Exposed)]
        [TestCase(3, VisualExposureState.Exposed)]
        public void VisibleSamplesClassifyIntoLockedExposureBands(
            int visibleSamples,
            VisualExposureState expected)
        {
            Assert.That(VisualConfirmationModel.Classify(visibleSamples), Is.EqualTo(expected));
        }

        [Test]
        public void ConfirmationUsesQuietMovingExposedSprintAndDecayRates()
        {
            var tuning = VisualConfirmationTuning.Default;
            var model = new VisualConfirmationModel(tuning);

            var quiet = model.Tick(1, true, 0.5f, false, 1f);
            Assert.That(quiet.RatePerSecond, Is.EqualTo(0.45f).Within(0.0001f));
            Assert.That(quiet.Confirmation, Is.EqualTo(0.45f).Within(0.0001f));

            model.Reset();
            var moving = model.Tick(1, false, 1f, false, 1f);
            Assert.That(moving.RatePerSecond, Is.EqualTo(0.65f).Within(0.0001f));

            model.Reset();
            var exposed = model.Tick(2, false, 1f, false, 0.5f);
            Assert.That(exposed.RatePerSecond, Is.EqualTo(1.35f).Within(0.0001f));
            Assert.That(exposed.Confirmation, Is.EqualTo(0.675f).Within(0.0001f));

            model.Reset();
            var sprint = model.Tick(2, false, 5f, true, 0.5f);
            Assert.That(sprint.RatePerSecond, Is.EqualTo(1.35f * 1.35f).Within(0.0001f));

            var concealed = model.Tick(0, false, 0f, false, 0.5f);
            Assert.That(concealed.RatePerSecond, Is.EqualTo(-1f).Within(0.0001f));
            Assert.That(concealed.Confirmation, Is.LessThan(sprint.Confirmation));
        }

        [Test]
        public void ConfirmationClampsConfirmsAndResets()
        {
            var model = new VisualConfirmationModel(VisualConfirmationTuning.Default);

            var confirmed = model.Tick(3, false, 2f, false, 1f);
            Assert.That(confirmed.Confirmation, Is.EqualTo(1f));
            Assert.That(confirmed.Confirmed, Is.True);

            model.Reset();
            Assert.That(model.Confirmation, Is.Zero);
            Assert.That(model.Exposure, Is.EqualTo(VisualExposureState.Concealed));
            Assert.That(model.IsConfirmed, Is.False);
        }

        [TestCase(0.149f, false)]
        [TestCase(0.15f, true)]
        [TestCase(0.151f, true)]
        public void HearingThresholdIsInclusiveAfterDistanceFalloff(float evaluatedStrength, bool accepted)
        {
            var stimulus = new ThreatStimulus(
                ThreatStimulusKind.Movement,
                Vector3.zero,
                evaluatedStrength,
                0f);

            var decision = ThreatHearingPolicy.Evaluate(stimulus, Vector3.zero);

            Assert.That(decision.Accepted, Is.EqualTo(accepted));
            Assert.That(decision.EvaluatedStrength, Is.EqualTo(evaluatedStrength).Within(0.0001f));
        }

        [Test]
        public void HearingEvaluatesDistanceBeforeThreshold()
        {
            var stimulus = new ThreatStimulus(
                ThreatStimulusKind.Sprint,
                Vector3.zero,
                0.30f,
                10f);

            Assert.That(ThreatHearingPolicy.Evaluate(stimulus, Vector3.right * 5f).Accepted, Is.True);
            Assert.That(ThreatHearingPolicy.Evaluate(stimulus, Vector3.right * 5.1f).Accepted, Is.False);
        }

        [Test]
        public void KnowledgePriorityKeepsSightAboveSoundAndRejectsMissingSound()
        {
            var visual = new Vector3(1f, 0f, 1f);
            var sound = new Vector3(8f, 0f, 8f);
            var lastSeen = new Vector3(2f, 0f, 2f);
            var probe = new Vector3(3f, 0f, 3f);
            var patrol = new Vector3(4f, 0f, 4f);

            var sight = ThreatKnowledgePolicy.Select(
                true, visual, true, sound, true, lastSeen, true, probe, patrol);
            Assert.That(sight.Source, Is.EqualTo(ThreatKnowledgeSource.ConfirmedCurrentVisual));
            Assert.That(sight.Position, Is.EqualTo(visual));

            var acceptedSound = ThreatKnowledgePolicy.Select(
                false, visual, true, sound, true, lastSeen, true, probe, patrol);
            Assert.That(acceptedSound.Source, Is.EqualTo(ThreatKnowledgeSource.AcceptedSound));
            Assert.That(acceptedSound.Position, Is.EqualTo(sound));

            var rejectedSound = ThreatKnowledgePolicy.Select(
                false, visual, false, sound, true, lastSeen, true, probe, patrol);
            Assert.That(rejectedSound.Source, Is.EqualTo(ThreatKnowledgeSource.LastConfirmedVisual));
            Assert.That(rejectedSound.Position, Is.EqualTo(lastSeen));
        }

        [TestCase(0f, false, false, MovementNoiseBand.Still)]
        [TestCase(0.4f, true, false, MovementNoiseBand.Crouch)]
        [TestCase(2f, false, false, MovementNoiseBand.Walk)]
        [TestCase(5f, false, true, MovementNoiseBand.Sprint)]
        public void MovementBandsUsePlanarMotionAndPosture(
            float speed,
            bool crouching,
            bool sprinting,
            MovementNoiseBand expected)
        {
            Assert.That(
                PlayerMovementNoisePolicy.Classify(speed, crouching, sprinting),
                Is.EqualTo(expected));
        }

        [Test]
        public void MovementProfilesAndPulseSchedulerUseLockedValuesWithoutStationaryEmission()
        {
            var crouch = PlayerMovementNoisePolicy.GetProfile(MovementNoiseBand.Crouch);
            var walk = PlayerMovementNoisePolicy.GetProfile(MovementNoiseBand.Walk);
            var sprint = PlayerMovementNoisePolicy.GetProfile(MovementNoiseBand.Sprint);
            Assert.That((crouch.PulseInterval, crouch.Intensity, crouch.Radius), Is.EqualTo((0.80f, 0.16f, 6f)));
            Assert.That((walk.PulseInterval, walk.Intensity, walk.Radius), Is.EqualTo((0.68f, 0.32f, 10f)));
            Assert.That((sprint.PulseInterval, sprint.Intensity, sprint.Radius), Is.EqualTo((0.45f, 0.62f, 15f)));

            var scheduler = new MovementNoisePulseScheduler();
            Assert.That(scheduler.TryCreatePulse(MovementNoiseBand.Still, 1f, out _), Is.False);
            Assert.That(scheduler.TryCreatePulse(MovementNoiseBand.Walk, 1.1f, out var pulse), Is.True);
            Assert.That(pulse.PulseInterval, Is.EqualTo(0.68f));
            Assert.That(scheduler.TryCreatePulse(MovementNoiseBand.Walk, 1.5f, out _), Is.False);
            Assert.That(scheduler.TryCreatePulse(MovementNoiseBand.Walk, 1.78f, out _), Is.True);

            scheduler.Reset();
            Assert.That(scheduler.NextPulseAt, Is.Zero);
        }
    }
}
