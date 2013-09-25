using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget.Operations
{
    [TestFixture]
    public class update_an_existing_floated_dependency_offline : NugetOperationContext
    {
        private SolutionScenario theScenario;
        private Solution theSolution;
        private NugetPlan thePlan;
        private NugetPlanBuilder theBuilder;
        private NugetPlanRequest theRequest;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionScenario.Create(scenario =>
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

            theRequest = new NugetPlanRequest
            {
                Solution = theSolution,
                Dependency = new Dependency("FubuCore"),
                Operation = OperationType.Update,
                ForceUpdates = false
            };
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void cannot_update_floats_in_offline_mode()
        {
            Exception<RippleFatalError>
                .ShouldBeThrownBy(() => theBuilder.PlanFor(theRequest));
        }

    }
}