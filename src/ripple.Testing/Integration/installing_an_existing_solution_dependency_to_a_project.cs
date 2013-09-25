using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class installing_an_existing_solution_dependency_to_a_project
    {
        private SolutionScenario theScenario;
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            FeedScenario.Create(scenario =>
            {
                scenario.For(Feed.NuGetV2)
                        .Add("FubuCore", "1.2.0.0");
            });

            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                {
                    test.SolutionDependency("FubuCore", "1.0.0.0", UpdateMode.Float);
                });
            });

            theSolution = theScenario.Find("Test");
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void installs_the_dependency()
        {
            RippleOperation
                .With(theSolution)
                .Execute<InstallInput, InstallCommand>(x =>
                {
                    x.Package = "FubuCore";
                    x.ProjectFlag = "Test";
                });

            theSolution = SolutionBuilder.ReadFrom(theScenario.DirectoryForSolution("Test"));

            theSolution.FindProject("Test").Dependencies.Has("FubuCore").ShouldBeTrue();
        }
    }
}