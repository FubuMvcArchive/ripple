using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class restoring_prelease_nugets_from_cache
    {
        private SolutionScenario theScenario;
        private Solution theSolution;
        private Feed nServiceBus;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("NServiceBus.WebSphereMQ", test =>
                {
                    test.SolutionDependency("NServiceBus.Interfaces", "4.0.0.0", UpdateMode.Float);
                });

                scenario.AddCachedNuget("NServiceBus.Interfaces", "4.0.0.0-unstable3041");
            });

            nServiceBus = new Feed("http://builds.nservicebus.com/guestAuth/app/nuget/v1/FeedService.svc", UpdateMode.Float, NugetStability.Anything);

            theSolution = theScenario.Find("NServiceBus.WebSphereMQ");
            theSolution.ClearFeeds();
            theSolution.AddFeed(nServiceBus);

            FeedScenario.Create(scenario =>
            {
                scenario.For(nServiceBus)
                        .Add("NServiceBus.Interfaces", "4.0.0.0-beta0002");
            });

            NugetFolderCache.DisableValidation();
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            NugetFolderCache.Reset();
        }

        [Test]
        public void restores_the_latest_version()
        {
            var missing = theSolution.MissingNugets().Single();

            var target = theSolution.Restore(missing);
            target.Wait();

            target.Result.Nuget.Version.ShouldEqual(new SemanticVersion("4.0.0.0-beta0002"));
        } 
    }
}