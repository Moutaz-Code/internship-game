using NUnit.Framework;

namespace InternshipGame.Prototype.Tests
{
    public sealed class FuelRodAssayStateModelTests
    {
        [Test]
        public void ContinuousThreeSecondHoldConfirmsLiveRod()
        {
            var assay = new FuelRodAssayStateModel();

            Assert.That(assay.Tick(2.9f, true, FuelRodClassification.Live), Is.False);
            Assert.That(assay.Tick(0.1f, true, FuelRodClassification.Live), Is.True);
            Assert.That(assay.State, Is.EqualTo(FuelRodAssayState.ConfirmedLive));
            Assert.That(assay.WasIdentified, Is.True);
        }

        [Test]
        public void ReleasingBeforeCompletionDiscardsProgress()
        {
            var assay = new FuelRodAssayStateModel();

            assay.Tick(2f, true, FuelRodClassification.Live);
            assay.Tick(0f, false, FuelRodClassification.Live);

            Assert.That(assay.State, Is.EqualTo(FuelRodAssayState.Unassayed));
            Assert.That(assay.ElapsedSeconds, Is.Zero);
            Assert.That(assay.NormalizedProgress, Is.Zero);
        }

        [Test]
        public void InertRodHasDistinctConfirmedStateAndCannotBeDeposited()
        {
            var assay = new FuelRodAssayStateModel();

            assay.Tick(3f, true, FuelRodClassification.Inert);

            Assert.That(assay.State, Is.EqualTo(FuelRodAssayState.ConfirmedInert));
            Assert.That(assay.MarkDeposited(), Is.False);
        }

        [Test]
        public void CompletedAssayDoesNotRegressWhenReleased()
        {
            var assay = new FuelRodAssayStateModel();
            assay.Tick(3f, true, FuelRodClassification.Live);

            assay.Tick(1f, false, FuelRodClassification.Live);

            Assert.That(assay.State, Is.EqualTo(FuelRodAssayState.ConfirmedLive));
        }
    }
}
