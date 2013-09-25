using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget.Operations
{
    [TestFixture]
    public class updating_to_a_higher_version_of_an_existing_fixed_solution_dependency : NugetOperationContext
    {
        private SolutionScenario theScenario;
        private Solution theSolution;
        private NugetPlan thePlan;
        private NugetPlanBuilder theBuilder;

        [SetUp]
        public void SetUp()
        {
            FeedScenario.Create(scenario => scenario.For(Feed.Fubu).Add("fubu", "1.2.0.0"));

            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                {
                    test.SolutionDependency("fubu", "1.0.0.1", UpdateMode.Fixed);
                    test.LocalDependency("fubu", "1.0.0.1");
                });
            });

            theSolution = theScenario.Find("Test");

            theBuilder = new NugetPlanBuilder();

            var request = new NugetPlanRequest
            {
                Solution = theSolution,
                Dependency = new Dependency("fubu"),
                Operation = OperationType.Update,
                ForceUpdates = false
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
        public void updates_the_solution_dependency()
        {
            thePlan.ShouldHaveTheSameElementsAs(
                updateSolutionDependency("fubu", "1.2.0.0", UpdateMode.Fixed)
            );
        }
    }
}