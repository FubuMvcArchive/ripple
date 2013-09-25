using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Commands;
using ripple.Model;
using ripple.Testing.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class restore_dependencies_that_only_exist_in_cache
    {
        private SolutionScenario theScenario;
        private Solution theSolution;
        private Feed theCache;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                {
                    test.SolutionDependency("TestNuGet", "1.1.0.1", UpdateMode.Float);
                    test.ProjectDependency("Test", "TestNuGet");
                });
            });

            theCache = new Feed("cache");

            theSolution = theScenario.Find("Test");
            theSolution.UseCache(new InMemoryNugetCache(theCache));

            FeedScenario.Create(scenario =>
            {
                scenario.For(theCache)
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
        public void restores_the_package()
        {
            var local = theSolution.LocalDependencies();
            local.Get("TestNuGet").Version.ShouldEqual(new SemanticVersion("1.1.0.1"));
        }
    }
}