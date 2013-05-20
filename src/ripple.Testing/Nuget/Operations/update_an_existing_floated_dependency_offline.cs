using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget.Operations
{
    [TestFixture]
    public class update_an_existing_floated_dependency_offline : NugetOperationContext
    {
        private SolutionGraphScenario theScenario;
        private Solution theSolution;
        private NugetPlan thePlan;
        private NugetPlanBuilder theBuilder;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionGraphScenario.Create(scenario =>
            {
                scenario.Solution("Test", sln =>
                {
                    sln.LocalDependency("FubuCore", "1.1.0.0");

                    sln.ProjectDependency("Test1", "FubuCore");
                });
            });

            theSolution = theScenario.Find("Test");

            FeedScenario.Create(scenario =>
            {
                scenario.For(theSolution.Cache.ToFeed())
                        .Add("FubuCore", "1.1.0.0")
                        .Add("FubuCore", "1.2.0.0");

                scenario.Offline();
            });

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
            FeedConnectivity.Live();
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