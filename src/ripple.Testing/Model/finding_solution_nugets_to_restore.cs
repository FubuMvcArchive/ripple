using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class finding_solution_nugets_to_restore
    {
        private Solution theSolution;

        private Dependency fubucore;
        private Dependency bottles;
        private Dependency rhinomocks;
        private Dependency structuremap;

        [SetUp]
        public void SetUp()
        {
            theSolution = new Solution();
            theSolution.ClearFeeds();

            theSolution.AddFeed(Feed.Fubu);
            theSolution.AddFeed(Feed.NuGetV2);

            theSolution.AddDependency(bottles = new Dependency("Bottles"));
            theSolution.AddDependency(fubucore = new Dependency("FubuCore", "1.0.1.201"));
            theSolution.AddDependency(rhinomocks = new Dependency("RhinoMocks", "3.6.1", UpdateMode.Fixed));
            theSolution.AddDependency(structuremap = new Dependency("StructureMap", "2.6.3", UpdateMode.Fixed));

            FeedScenario.Create(scenario =>
            {
                scenario.For(Feed.Fubu)
                    .Add("Bottles", "1.0.2.2")
                    .Add("FubuCore", "1.0.2.232")
                    .Add("StructureMap", "2.6.4.71");

                scenario.For(Feed.NuGetV2)
                    .Add("RhinoMocks", "3.6.1")
                    .Add("StructureMap", "2.6.3");

                scenario.For(theSolution.Cache.ToFeed());

                scenario.Online();
            });
        }

        [TearDown]
        public void TearDown()
        {
            FeedRegistry.Reset();
        }

        private void theVersionIs(Dependency dependency, string version)
        {
            var task = theSolution.Restore(dependency);
            task.Wait();

            var nuget = task.Result.Nuget;
            nuget.Version.ShouldEqual(new SemanticVersion(version));
        }

        [Test]
        public void finds_floated_nuget_without_a_version()
        {
            theVersionIs(bottles, "1.0.2.2");
        }

        [Test]
        public void finds_latest_version_from_floated_feed()
        {
            theVersionIs(fubucore, "1.0.2.232");
        }

        [Test]
        public void restores_the_fixed_version()
        {
            theVersionIs(rhinomocks, "3.6.1.0");
            theVersionIs(structuremap, "2.6.3");
        }
    }
}