using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget
{
    [TestFixture]
    public class EmptyVersionFinderTester
    {
        private EmptyVersionFinder theFinder;
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            theFinder = new EmptyVersionFinder();

            FeedScenario.Create(scenario =>
            {
                scenario.For(Feed.Fubu)
                        .Add("Test", "1.1.0.0")
                        .Add("Test", "1.2.0.0")
                        .Add("Test", "1.2.0.12");
            });

            theSolution = Solution.Empty();
            theSolution.AddFeed(Feed.Fubu);
        }

        [Test]
        public void matches_dependencies_with_no_version()
        {
            theFinder.Matches(new Dependency("Test")).ShouldBeTrue();
        }

        [Test]
        public void does_not_match_dependencies_with_verions()
        {
            theFinder.Matches(new Dependency("Test", "1.1.0.0")).ShouldBeFalse();
        }

        [Test]
        public void finds_the_latest_version()
        {
            var result = theFinder.Find(theSolution, new Dependency("Test"));

            result.Found.ShouldBeTrue();
            result.Nuget.Version.ShouldEqual(new SemanticVersion("1.2.0.12"));
        }
    }
}