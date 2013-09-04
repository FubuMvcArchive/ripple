using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Commands;
using ripple.Model;
using ripple.Testing.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class restore_float_dependency_with_no_version
    {
        private SolutionGraphScenario theScenario;
        private Solution theSolution;
        private Feed theCache;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionGraphScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                {
                    // Defacto a float
                    test.ProjectDependency("Test", "TestNuGet");
                });
            });

            theCache = new Feed("cache");

            theSolution = theScenario.Find("Test");
            theSolution.UseCache(new InMemoryNugetCache(theCache));

            FeedScenario.Create(scenario =>
            {
                scenario.For(theCache)
                    .Add("TestNuGet", "1.0.0.0");

                scenario.For(Feed.Fubu)
                    .Add("TestNuGet", "1.1.0.1");
            });

            RippleOperation
                .With(theSolution)
                .Execute<RestoreInput, RestoreCommand>();
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void uses_the_latest_from_the_floating_feed()
        {
            var local = theSolution.LocalDependencies();
            local.Get("TestNuGet").Version.ShouldEqual(new SemanticVersion("1.1.0.1"));
        }
    }
}