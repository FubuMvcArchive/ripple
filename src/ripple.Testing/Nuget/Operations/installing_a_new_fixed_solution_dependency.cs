using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget.Operations
{
    [TestFixture]
    public class installing_a_new_fixed_solution_dependency : NugetOperationContext
    {
        private Solution theSolution;
        private NugetPlan thePlan;
        private NugetPlanBuilder theBuilder;
        private SolutionScenario theScenario;

        [SetUp]
        public void SetUp()
        {
            FeedScenario.Create(scenario => scenario.For(Feed.NuGetV2).Add("fubu", "1.2.0.0"));

            theScenario = SolutionScenario.Create(scenario => scenario.Solution("Test"));
            theSolution = theScenario.Find("Test");

            theBuilder = new NugetPlanBuilder();

            var request = new NugetPlanRequest
            {
                Solution = theSolution,
                Dependency = new Dependency("fubu", "1.2.0.0", UpdateMode.Fixed),
                Operation = OperationType.Install
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
        public void installs_as_fixed_to_solution()
        {
            thePlan.ShouldHaveTheSameElementsAs(
                solutionInstallation("fubu", "1.2.0.0", UpdateMode.Fixed)
            );
        }
    }
}