using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget.Operations
{
    [TestFixture]
    public class installing_a_fixed_project_dependency_any_stability : NugetOperationContext
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
                scenario.For(Feed.NuGetV2)
                        .Add("FubuCore", "1.2.0.0-alpha");
            });

            theScenario = SolutionScenario.Create(scenario => scenario.Solution("Test"));

            theSolution = theScenario.Find("Test");

            theBuilder = new NugetPlanBuilder();

            var request = new NugetPlanRequest
                {
                    Solution = theSolution,
                    Dependency = new Dependency("FubuCore", "1.2.0.0", UpdateMode.Fixed) { NugetStability = NugetStability.Anything },
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
        public void verify_plan()
        {
            thePlan.ShouldHaveTheSameElementsAs(
                solutionInstallation("FubuCore", "1.2.0.0", UpdateMode.Fixed),
                projectInstallation("Test", "FubuCore")
            );
        }
    }
}