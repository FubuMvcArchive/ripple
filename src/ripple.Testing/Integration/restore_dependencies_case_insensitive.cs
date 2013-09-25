using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class restore_dependencies_case_insensitive
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
                    test.SolutionDependency("StructureMap", "2.6.3", UpdateMode.Fixed);
                    test.ProjectDependency("Test", "structuremap");
                });
            });

            theSolution = theScenario.Find("Test");

            FeedScenario.Create(scenario =>
            {
                scenario.For(Feed.Fubu)
                        .Add("structuremap", "2.6.4.54");

                scenario.For(Feed.NuGetV2)
                        .Add("structuremap", "2.6.3");
            });

            RippleOperation
                .With(theSolution)
                .Execute<RestoreInput, RestoreCommand>();
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
        }

        [Test]
        public void restores_the_fixed_version()
        {
            var local = theSolution.LocalDependencies();
            local.Get("structuremap").Version.ShouldEqual(new SemanticVersion("2.6.3"));
        }
    }
}