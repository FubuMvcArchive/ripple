using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class updating_a_dependency_with_transitive_updates
    {
        private SolutionScenario theScenario;
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            FeedScenario.Create(scenario =>
            {
                scenario
                    .For(Feed.Fubu)
                    .Add("Serenity", "1.0.0.2")
                    .Add("Something", "1.0.0.0")
                    .Add("FubuCore", "1.0.0.1")
                    .ConfigureRepository(teamcity =>
                    {
                        teamcity.ConfigurePackage("Serenity", "1.0.0.2", serenity =>
                        {
                            serenity.DependsOn("WebDriver", "1.2.0.0");
                            serenity.DependsOn("Something");
                            serenity.DependsOn("SomethingElse", "0.9.9.9");
                        });
                    });

                scenario.For(Feed.NuGetV2)
                    .Add("WebDriver", "1.2.0.0")
                    .Add("SomethingElse", "0.9.9.9");
            });

            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                {
                    test.LocalDependency("WebDriver", "1.1.0.0");

                    test.ProjectDependency("Test", "Serenity");
                    test.ProjectDependency("Test", "WebDriver");

                    test.ProjectDependency("Test2", "FubuCore");

                    test.SolutionDependency("WebDriver", "1.1.0.0", UpdateMode.Fixed);
                });
            });

            theSolution = theScenario.Find("Test");

            RippleOperation
                .With(theSolution)
                .Execute<UpdateInput, UpdateCommand>(input =>
                {
                    input.NugetFlag = "Serenity";
                    input.ForceFlag = true;
                });
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void updates_the_configured_dependency()
        {
            theSolution.Dependencies.Find("WebDriver").Version.ShouldEqual("1.2.0.0");
        }

        [Test]
        public void keeps_the_configured_dependency_fixed()
        {
            theSolution.Dependencies.Find("WebDriver").Mode.ShouldEqual(UpdateMode.Fixed);
        }

        [Test]
        public void updates_the_local_dependency()
        {
            theSolution.LocalDependencies().Get("WebDriver").Version.ShouldEqual(new SemanticVersion("1.2.0.0"));
        }

        [Test]
        public void installs_the_floated_transitive_dependency()
        {
            theSolution.Dependencies.Find("Something").Mode.ShouldEqual(UpdateMode.Float);
        }

        [Test]
        public void installs_the_floated_transitive_dependencies_to_the_project()
        {
            theSolution.FindProject("Test").Dependencies.Has("Something").ShouldBeTrue();
            theSolution.FindProject("Test2").Dependencies.Has("Something").ShouldBeFalse();
        }

        [Test]
        public void installs_the_fixed_transitive_dependencies_to_the_project()
        {
            theSolution.FindProject("Test").Dependencies.Has("SomethingElse").ShouldBeTrue();
            theSolution.FindProject("Test2").Dependencies.Has("SomethingElse").ShouldBeFalse();
        }
    }
}