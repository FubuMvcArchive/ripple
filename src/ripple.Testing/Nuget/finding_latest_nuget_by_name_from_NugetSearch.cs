using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget
{
    [TestFixture]
    public class finding_latest_nuget_by_name_from_NugetSearch
    {
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
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
        public void finds_the_latest_version()
        {
            var result = NugetSearch.FindLatestByName(theSolution, "Test");

            result.Found.ShouldBeTrue();
            result.Nuget.Version.ShouldEqual(new SemanticVersion("1.2.0.12"));
        }
    }
}