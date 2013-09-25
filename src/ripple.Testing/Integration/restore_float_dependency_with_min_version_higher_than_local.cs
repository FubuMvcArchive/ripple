using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class restore_float_dependency_with_min_version_higher_than_local
    {
        private SolutionScenario theScenario;
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                {
                    test.SolutionDependency("TestNuGet", "1.1.0.1", UpdateMode.Float);
                    test.ProjectDependency("Test", "TestNuGet");

                    test.LocalDependency("TestNuGet", "1.0.0.0");
                });
            });

            theSolution = theScenario.Find("Test");

            FeedScenario.Create(scenario =>
            {
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
        public void restores_the_min_version()
        {
            var local = theSolution.LocalDependencies();
            local.Get("TestNuGet").Version.ShouldEqual(new SemanticVersion("1.1.0.1"));
        }
    }
}