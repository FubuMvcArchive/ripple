using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class updating_an_existing_dependency_in_a_group
    {
        private SolutionScenario theScenario;
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            FeedScenario.Create(scenario =>
            {
                scenario.For(Feed.NuGetV2)
                        .Add("RavenDB.Client", "2.0.2330")
                        .Add("RavenDB.Database", "2.0.2330")
                        .Add("RavenDB.Embedded", "2.0.2330")
                        .Add("RavenDB.Server", "2.0.2330")
                        .ConfigureRepository(nuget =>
                        {
                            nuget.ConfigurePackage("RavenDB.Embedded", "2.0.2330", embedded =>
                            {
                                embedded.DependsOn("RavenDB.Client", "2.0.2330");
                                embedded.DependsOn("RavenDB.Database", "2.0.2330");
                            });
                        });
            });

            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", sln =>
                {
                    sln.LocalDependency("RavenDB.Client", "2.0.2315");
                    sln.LocalDependency("RavenDB.Database", "2.0.2315");
                    sln.LocalDependency("RavenDB.Embedded", "2.0.2315");
                    sln.LocalDependency("RavenDB.Server", "2.0.2315");

                    sln.SolutionDependency("RavenDB.Client", "2.0.2315", UpdateMode.Fixed);
                    sln.SolutionDependency("RavenDB.Database", "2.0.2315", UpdateMode.Fixed);
                    sln.SolutionDependency("RavenDB.Embedded", "2.0.2315", UpdateMode.Fixed);
                    sln.SolutionDependency("RavenDB.Server", "2.0.2315", UpdateMode.Fixed);

                    sln.ProjectDependency("Test", "RavenDB.Client");
                    sln.ProjectDependency("Test", "RavenDB.Database");
                    sln.ProjectDependency("Test", "RavenDB.Embedded");
                    sln.ProjectDependency("Test", "RavenDB.Server");

                    sln.GroupDependencies("RavenDB.Client", "RavenDB.Database", "RavenDB.Embedded", "RavenDB.Server");
                });
            });

            theSolution = theScenario.Find("Test");

            RippleOperation
                .With(theSolution)
                .Execute<UpdateInput, UpdateCommand>(input =>
                {
                    input.NugetFlag = "RavenDB.Client";
                });

            theSolution = SolutionBuilder.ReadFrom(theScenario.DirectoryForSolution("Test"));
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void verify_updates_of_all_grouped_items()
        {
            theSolution.Dependencies.Find("RavenDB.Client").SemanticVersion().ShouldEqual(new SemanticVersion("2.0.2330"));
            theSolution.Dependencies.Find("RavenDB.Database").SemanticVersion().ShouldEqual(new SemanticVersion("2.0.2330"));
            theSolution.Dependencies.Find("RavenDB.Embedded").SemanticVersion().ShouldEqual(new SemanticVersion("2.0.2330"));
            theSolution.Dependencies.Find("RavenDB.Server").SemanticVersion().ShouldEqual(new SemanticVersion("2.0.2330"));
        }
    }
}