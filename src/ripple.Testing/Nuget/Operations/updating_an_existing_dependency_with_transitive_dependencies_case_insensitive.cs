using System.Diagnostics;
using FubuCore.Descriptions;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget.Operations
{
    // This one will bite us in EVERY SINGLE repository so make sure it's always passing
    [TestFixture]
    public class updating_an_existing_dependency_with_transitive_dependencies_case_insensitive : NugetOperationContext
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
                        .Add("FubuTestingSupport", "1.1.0.0")
                        .Add("FubuTestingSupport", "1.2.0.0")
                        .Add("StructureMap", "2.6.4") // this just makes the test flow easier
                        .ConfigureRepository(fubu =>
                        {
                            fubu.ConfigurePackage("FubuTestingSupport", "1.2.0.0", support =>
                            {
                                support.DependsOn("StructureMap").Min("2.6.4");
                            });
                        });

                scenario.For(Feed.NuGetV2)
                    .Add("structuremap", "2.6.3");
            });

            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", sln =>
                {
                    sln.LocalDependency("FubuTestingSupport", "1.1.0.0");
                    sln.LocalDependency("structuremap", "2.6.3");

                    sln.SolutionDependency("structuremap", "2.6.3", UpdateMode.Fixed);

                    sln.ProjectDependency("Test1", "FubuTestingSupport");
                    sln.ProjectDependency("Test1", "structuremap");
                });
            });

            theSolution = theScenario.Find("Test");

            theBuilder = new NugetPlanBuilder();

            var request = new NugetPlanRequest
            {
                Solution = theSolution,
                Dependency = new Dependency("FubuTestingSupport"),
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
            Debug.WriteLine(thePlan.ToDescriptionText());
            thePlan.ShouldHaveTheSameElementsAs(
                updateSolutionDependency("FubuTestingSupport", "1.2.0.0", UpdateMode.Float)
            );
        }
    }
}