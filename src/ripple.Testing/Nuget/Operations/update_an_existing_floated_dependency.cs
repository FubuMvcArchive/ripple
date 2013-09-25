using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget.Operations
{
    [TestFixture]
    public class update_an_existing_floated_dependency : NugetOperationContext
    {
        private SolutionScenario theScenario;
        private Solution theSolution;
        private NugetPlan thePlan;
        private NugetPlanBuilder theBuilder;

        [SetUp]
        public void SetUp()
        {
            FeedScenario.Create(scenario =>
                {
                    scenario.For(Feed.Fubu)
                            .Add("FubuCore", "1.1.0.0")
                            .Add("FubuCore", "1.2.0.0");
                });

            theScenario = SolutionScenario.Create(scenario =>
                {
                    scenario.Solution("Test", sln =>
                        {
                            sln.LocalDependency("FubuCore", "1.1.0.0");

                            sln.ProjectDependency("Test1", "FubuCore");
                        });
                });

            theSolution = theScenario.Find("Test");

            theBuilder = new NugetPlanBuilder();

            var request = new NugetPlanRequest
                {
                    Solution = theSolution,
                    Dependency = new Dependency("FubuCore"),
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
        public void verify_plan()
        {
            thePlan.ShouldHaveTheSameElementsAs(
                updateSolutionDependency("FubuCore", "1.2.0.0", UpdateMode.Float)
                );
        }

    }
}