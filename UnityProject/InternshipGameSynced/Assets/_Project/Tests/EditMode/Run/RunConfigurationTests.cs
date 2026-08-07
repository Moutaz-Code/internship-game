using System.Linq;
using NUnit.Framework;

namespace InternshipGame.Prototype.Tests
{
    public sealed class RunConfigurationTests
    {
        [Test]
        public void SameSeedReproducesEverySelection()
        {
            var first = RunConfiguration.Generate(91827);
            var second = RunConfiguration.Generate(91827);

            Assert.That(second.LiveSocketIds, Is.EqualTo(first.LiveSocketIds));
            Assert.That(second.InertSocketIds, Is.EqualTo(first.InertSocketIds));
            Assert.That(second.RouteProfile, Is.EqualTo(first.RouteProfile));
            Assert.That(second.OwlProfile, Is.EqualTo(first.OwlProfile));
            Assert.That(second.LightingProfile, Is.EqualTo(first.LightingProfile));
        }

        [Test]
        public void ConfigurationSelectsOneLiveRodPerLobeAndTwoUniqueUnusedDecoys()
        {
            for (var seed = -20; seed <= 20; seed++)
            {
                var configuration = RunConfiguration.Generate(seed);

                Assert.That(configuration.LiveSocketIds, Has.Count.EqualTo(3));
                Assert.That(configuration.InertSocketIds, Has.Count.EqualTo(2));
                Assert.That(configuration.LiveSocketIds.Distinct().Count(), Is.EqualTo(3));
                Assert.That(configuration.InertSocketIds.Distinct().Count(), Is.EqualTo(2));
                Assert.That(configuration.LiveSocketIds.Intersect(configuration.InertSocketIds), Is.Empty);
                Assert.That(
                    configuration.LiveSocketIds.Count(id => id.StartsWith("C")),
                    Is.EqualTo(1));
                Assert.That(
                    configuration.LiveSocketIds.Count(id => id.StartsWith("K")),
                    Is.EqualTo(1));
                Assert.That(
                    configuration.LiveSocketIds.Count(id => id.StartsWith("F")),
                    Is.EqualTo(1));
            }
        }
    }
}
