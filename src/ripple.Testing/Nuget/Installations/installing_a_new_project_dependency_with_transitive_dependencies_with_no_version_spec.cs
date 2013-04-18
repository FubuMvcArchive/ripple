using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget.Installations
{
    [TestFixture]
    public class installing_a_new_project_dependency_with_transitive_dependencies_with_no_version_spec : NugetPlanContext
    {
        private Solution theSolution;
        private NugetPlan thePlan;
        private NugetPlanBuilder theBuilder;

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
                                katana.AddDependency("FubuMVC.Core");
                                katana.AddDependency("FubuMVC.OwinHost");
                            });

                            teamcity.ConfigurePackage("FubuMVC.OwinHost", "1.2.0.0", owin => owin.AddDependency("FubuMVC.Core"));
                            teamcity.ConfigurePackage("FubuMVC.OwinHost", "1.3.0.0", owin => owin.AddDependency("FubuMVC.Core"));
                        });
            });

            theSolution = new Solution();
            theSolution.AddProject("Test");

            theBuilder = new NugetPlanBuilder();

            var request = new NugetPlanRequest
            {
                Solution = theSolution,
                Dependency = new Dependency("FubuMVC.Katana"),
                Operation = OperationType.Install,
                Project = "Test",
                Mode = UpdateMode.Float
            };

            thePlan = theBuilder.PlanFor(request);
        }

        [TearDown]
        public void TearDown()
        {
            FeedRegistry.Reset();
        }

        [Test]
        public void installs_the_latest_versions()
        {
            thePlan.ShouldHaveTheSameElementsAs(

                solutionInstallation("FubuMVC.Katana", "1.0.0.1", UpdateMode.Float),
                solutionInstallation("FubuMVC.Core", "1.1.0.2", UpdateMode.Float),
                solutionInstallation("FubuMVC.OwinHost", "1.3.0.0", UpdateMode.Float),

                projectInstallation("Test", "FubuMVC.Katana"),
                projectInstallation("Test", "FubuMVC.Core"),
                projectInstallation("Test", "FubuMVC.OwinHost")
            );
        }
    }
}