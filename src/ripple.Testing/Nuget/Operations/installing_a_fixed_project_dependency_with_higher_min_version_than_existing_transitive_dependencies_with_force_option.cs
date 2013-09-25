using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget.Operations
{
    [TestFixture]
    public class installing_a_fixed_project_dependency_with_higher_min_version_than_existing_transitive_dependencies_with_force_option : NugetOperationContext
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
                        .Add("FubuCore", "1.2.0.0")
                        .Add("FubuCore", "1.3.0.0")
                        .Add("Bottles", "1.1.0.0")
                        .ConfigureRepository(nuget =>
                        {
                            nuget.ConfigurePackage("Bottles", "1.1.0.0", bottles => bottles.DependsOn("FubuCore").Min("1.3.0.0"));
                        });
            });

            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                {
                    test.ProjectDependency("Test", "FubuCore");
                    test.LocalDependency("FubuCore", "1.2.0.0");
                });
            });

            theSolution = theScenario.Find("Test");

            theBuilder = new NugetPlanBuilder();

            var request = new NugetPlanRequest
            {
                Solution = theSolution,
                Dependency = new Dependency("Bottles", "1.1.0.0", UpdateMode.Fixed),
                Operation = OperationType.Install,
                ForceUpdates = true,
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
        public void installs_the_new_package_and_updates_the_existing()
        {
            thePlan.ShouldHaveTheSameElementsAs(
                solutionInstallation("Bottles", "1.1.0.0", UpdateMode.Fixed),
                projectInstallation("Test", "Bottles"),
                updateSolutionDependency("FubuCore", "1.3.0.0", UpdateMode.Fixed)
            );
        }
    }
}