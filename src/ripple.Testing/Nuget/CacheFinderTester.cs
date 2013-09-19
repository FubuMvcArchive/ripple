using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;
using ripple.Nuget;
using ripple.Testing.Model;

namespace ripple.Testing.Nuget
{
    [TestFixture]
    public class CacheFinderTester
    {
        private CacheFinder theFinder;
        private Feed theCache;
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            theFinder = new CacheFinder();
            theCache = new Feed("cache");

            FeedScenario.Create(scenario =>
            {
                scenario.For(theCache)
                    .Add("Test", "1.1.0.0");
            });

            theSolution = Solution.Empty();
            theSolution.UseCache(new InMemoryNugetCache(theCache));
        }

        [TearDown]
        public void TearDown()
        {
            FeedRegistry.Reset();
        }

        [Test]
        public void does_not_match_when_no_version_is_specified()
        {
            theFinder.Matches(new Dependency("Test")).ShouldBeFalse();
        }

        [Test]
        public void matches_dependencies_with_versions()
        {
            theFinder.Matches(new Dependency("Test", "1.0.0.0")).ShouldBeTrue();
            theFinder.Matches(new Dependency("Test", "1.0.0.0", UpdateMode.Float)).ShouldBeTrue();
        }

        [Test]
        public void finds_the_nuget_from_the_cache()
        {
            var result = theFinder.Find(theSolution, new Dependency("Test", "1.1.0.0"));

            result.Found.ShouldBeTrue();
            result.Nuget.Version.ShouldEqual(new SemanticVersion("1.1.0.0"));
        }
    }
}