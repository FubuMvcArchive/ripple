using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget
{
    [TestFixture]
    public class DefaultFinderTester
    {
        private DefaultFinder theFinder;
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            theFinder = new DefaultFinder();

            FeedScenario.Create(scenario =>
            {
                scenario.For(Feed.NuGetV2)
                        .Add("Test", "1.1.0.0")
                        .Add("Test", "1.2.0.0")
                        .Add("Test", "1.2.0.12");
            });

            theSolution = Solution.Empty();
            theSolution.AddFeed(Feed.NuGetV2);
        }

        [Test]
        public void matches_all_dependencies()
        {
            theFinder.Matches(new Dependency("Test", "1.1.0.0", UpdateMode.Fixed)).ShouldBeTrue();
            theFinder.Matches(null).ShouldBeTrue();
        }

        [Test]
        public void finds_the_version()
        {
            var result = theFinder.Find(theSolution, new Dependency("Test", "1.2.0.12", UpdateMode.Fixed));

            result.Found.ShouldBeTrue();
            result.Nuget.Version.ShouldEqual(new SemanticVersion("1.2.0.12"));
        }

        [Test]
        public void filter_a_floated_result_that_is_older_than_the_newest_in_the_feed()
        {
            var result = NugetResult.For(new StubNuget("Test", "1.0.0.0"));
            theFinder.Filter(theSolution, new Dependency("Test"), result);

            result.Nuget.Version.ShouldEqual(new SemanticVersion("1.2.0.12"));
        }

        [Test]
        public void filter_a_fixed_result_that_is_older_than_the_newest_in_the_feed_does_nothing()
        {
            var result = NugetResult.For(new StubNuget("Test", "1.0.0.0"));
            theFinder.Filter(theSolution, new Dependency("Test", "1.0.0.0", UpdateMode.Fixed), result);

            result.Nuget.Version.ShouldEqual(new SemanticVersion("1.0.0.0"));
        }
    }
}