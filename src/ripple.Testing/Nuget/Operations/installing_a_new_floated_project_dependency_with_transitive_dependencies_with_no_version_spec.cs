using System.Diagnostics;
using FubuCore.Descriptions;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget.Operations
{
    [TestFixture]
    public class installing_a_new_floated_project_dependency_with_transitive_dependencies_with_no_version_spec : NugetOperationContext
    {
        private Solution theSolution;
        private NugetPlan thePlan;
        private NugetPlanBuilder theBuilder;
        private SolutionScenario theScenario;

        [SetUp]
        public void SetUp()
        {
            FeedScenario.Create(scenario =>
            {
                scenario.For(Feed.Fubu)
                        .Add("FubuMVC.Katana", "1.0.0.1")
                        .Add("FubuMVC.Core", "1.0.1.1")
                        .Add("FubuMVC.Core", "1.1.0.2")
                        .Add("FubuMVC.OwinHost", "1.2.0.0")
                        .Add("FubuMVC.OwinHost", "1.3.0.0")
                        .ConfigureRepository(teamcity =>
                        {
                            teamcity.ConfigurePackage("FubuMVC.Katana", "1.0.0.1", katana =>
                            {
                                katana.DependsOn("FubuMVC.Core");
                                katana.DependsOn("FubuMVC.OwinHost");
                            });

                            teamcity.ConfigurePackage("FubuMVC.OwinHost", "1.2.0.0", owin => owin.DependsOn("FubuMVC.Core"));
                            teamcity.ConfigurePackage("FubuMVC.OwinHost", "1.3.0.0", owin =>
	                        {
		                        owin.DependsOn("FubuMVC.Core");
		                        owin.DependsOn("FixedNuget");
	                        });
                        });

	            scenario.For(Feed.NuGetV2)
	                    .Add("FixedNuget", "1.0.0.0");
            });

            theScenario = SolutionScenario.Create(scenario => scenario.Solution("Test"));
            theSolution = theScenario.Find("Test");

            theBuilder = new NugetPlanBuilder();

            var request = new NugetPlanRequest
            {
                Solution = theSolution,
                Dependency = new Dependency("FubuMVC.Katana", UpdateMode.Float),
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
        public void installs_the_latest_versions()
        {
            Debug.WriteLine(thePlan.ToDescriptionText());

            thePlan.ShouldHaveTheSameElementsAs(

                solutionInstallation("FubuMVC.Katana", "1.0.0.1", UpdateMode.Float),
                projectInstallation("Test", "FubuMVC.Katana"),

				solutionInstallation("FixedNuget", "1.0.0.0", UpdateMode.Fixed),
				projectInstallation("Test", "FixedNuget"),

                solutionInstallation("FubuMVC.Core", "1.1.0.2", UpdateMode.Float),
                projectInstallation("Test", "FubuMVC.Core"),

                solutionInstallation("FubuMVC.OwinHost", "1.3.0.0", UpdateMode.Float),
                projectInstallation("Test", "FubuMVC.OwinHost")
            );
        }
    }
}