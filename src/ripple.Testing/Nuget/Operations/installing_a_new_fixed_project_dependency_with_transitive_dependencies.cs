using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget.Operations
{
    [TestFixture]
    public class installing_a_new_fixed_project_dependency_with_transitive_dependencies_with_no_version_specs : NugetOperationContext
    {
        private Solution theSolution;
        private NugetPlan thePlan;
        private NugetPlanBuilder theBuilder;
        private SolutionScenario theScenario;

        [SetUp]
        public void SetUp()
        {
            FeedScenario.Create(scenario => scenario.For(Feed.Fubu).Add("FubuCore", "1.0.0.1"));
            FeedScenario.Create(scenario =>
            {
                scenario.For(Feed.NuGetV2)
                        .Add("TopShelf", "1.1.0.0")
                        .Add("log4net", "1.0.0.5")
                        .Add("log4net", "1.0.1.1")
                        .ConfigureRepository(nuget =>
                        {
                            nuget.ConfigurePackage("TopShelf", "1.1.0.0", topshelf => topshelf.DependsOn("log4net"));
                        });
            });

            theScenario = SolutionScenario.Create(scenario => scenario.Solution("Test"));
            theSolution = theScenario.Find("Test");

            theBuilder = new NugetPlanBuilder();

            var request = new NugetPlanRequest
            {
                Solution = theSolution,
                Dependency = new Dependency("TopShelf", "1.1.0.0", UpdateMode.Fixed),
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
        public void installs_the_project_dependency_and_marks_solution_level_info_using_latest_versions()
        {
            thePlan.ShouldHaveTheSameElementsAs(

                solutionInstallation("TopShelf", "1.1.0.0", UpdateMode.Fixed),
                projectInstallation("Test", "TopShelf"),
                
                solutionInstallation("log4net", "1.0.1.1", UpdateMode.Fixed),
                projectInstallation("Test", "log4net")
            );
        }
    }
}