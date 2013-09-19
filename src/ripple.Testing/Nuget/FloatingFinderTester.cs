using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget
{
    [TestFixture]
    public class FloatingFinderTester
    {
        private FloatingFinder theFinder;
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            theFinder = new FloatingFinder();

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
        public void matches_floated_dependencies()
        {
            theFinder.Matches(new Dependency("Test")).ShouldBeTrue();
        }

        [Test]
        public void does_not_match_fixed_dependencies()
        {
            theFinder.Matches(new Dependency("Test", "1.1.0.0", UpdateMode.Fixed)).ShouldBeFalse();
        }

        [Test]
        public void finds_the_latest_version()
        {
            var result = theFinder.Find(theSolution, new Dependency("Test", "1.1.0.0"));

            result.Found.ShouldBeTrue();
            result.Nuget.Version.ShouldEqual(new SemanticVersion("1.2.0.12"));
        }

        [Test]
        public void find_the_latest_for_a_fixed_dependency_should_respect_the_upper_bounds()
        {
            var result = theFinder.Find(theSolution, new Dependency("Test", "1.0.0.0", UpdateMode.Fixed));;

            result.Found.ShouldBeFalse();
        }
    }
}