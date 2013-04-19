using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget.Operations
{
    [TestFixture]
    public class installing_a_new_fixed_project_dependency_with_no_transitive_dependencies : NugetOperationContext
    {
        private Solution theSolution;
        private NugetPlan thePlan;
        private NugetPlanBuilder theBuilder;
        private SolutionGraphScenario theScenario;

        [SetUp]
        public void SetUp()
        {
            FeedScenario.Create(scenario => scenario.For(Feed.Fubu).Add("FubuCore", "1.0.0.1"));

            theScenario = SolutionGraphScenario.Create(scenario => scenario.Solution("Test"));

            theSolution = theScenario.Find("Test");

            theBuilder = new NugetPlanBuilder();

            var request = new NugetPlanRequest
            {
                Solution = theSolution,
                Dependency = new Dependency("FubuCore", "1.0.0.1", UpdateMode.Fixed),
                Operation = OperationType.Install,
                Project = "Test"
            };

            thePlan = theBuilder.PlanFor(request);
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void installs_the_project_dependency_and_marks_solution_level_info()
        {
            thePlan.ShouldHaveTheSameElementsAs(

                solutionInstallation("FubuCore", "1.0.0.1", UpdateMode.Fixed),

                projectInstallation("Test", "FubuCore")
            );
        }
    }
}