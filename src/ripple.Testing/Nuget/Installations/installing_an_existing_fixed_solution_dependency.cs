using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget.Installations
{
    [TestFixture]
    public class installing_an_existing_fixed_solution_dependency
    {
        private SolutionGraphScenario theScenario;
        private Solution theSolution;
        private NugetPlan thePlan;
        private NugetPlanBuilder theBuilder;

        [SetUp]
        public void SetUp()
        {
            FeedScenario.Create(scenario => scenario.For(Feed.Fubu).Add("fubu", "1.2.0.0"));

            theScenario = SolutionGraphScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                {
                    test.LocalDependency("fubu", "1.2.0.0");
                });
            });

            theSolution = theScenario.Find("Test");

            theBuilder = new NugetPlanBuilder();

            var request = new NugetPlanRequest
            {
                Solution = theSolution,
                Dependency = new Dependency("fubu", "1.2.0.0"),
                Operation = OperationType.Install,
                Mode = UpdateMode.Fixed
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
        public void no_installation()
        {
            thePlan.ShouldHaveCount(0);
        }
    }
}