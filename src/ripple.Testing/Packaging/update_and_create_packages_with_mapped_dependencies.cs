using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;
using ripple.Nuget;
using ripple.Packaging;

namespace ripple.Testing.Packaging
{
    [TestFixture]
    public class update_and_create_packages_with_mapped_dependencies
    {
        private SolutionScenario theScenario;
        private Solution theSolution;
        private string theOutputDir;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("FubuCore", test =>
                {
                    // No, I don't like this name more. I'm just making it complicated
                    test.Publishes("FubuFoundation", x => x.Assembly("FubuCore.dll"));
                    test.Publishes("FubuCore.Interfaces", x => x.Assembly("FubuCore.Interfaces.dll", "FubuCore"));
                });
            });

            theOutputDir = theScenario.CreateDirectory("output");

            theSolution = theScenario.Find("FubuCore");
            theSolution.AddNuspec(new NuspecMap { PackageId = "FubuCore.Interfaces", PublishedBy = "FubuCore" });
            theSolution.AddNuspec(new NuspecMap { PackageId = "FubuFoundation", PublishedBy = "FubuCore", DependsOn = "FubuCore.Interfaces" });

            RippleOperation
                .With(theSolution)
                .Execute<CreatePackagesInput, CreatePackagesCommand>(x =>
                {
                    x.UpdateDependenciesFlag = true;
                    x.DestinationFlag = theOutputDir;
                    x.VersionFlag = "1.1.0.0";
                });
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
        }

        [Test]
        public void creates_the_packages()
        {
            var feed = new FileSystemNugetFeed(theOutputDir, NugetStability.Anything);
            feed.FindLatestByName("FubuFoundation").Version.ShouldEqual(new SemanticVersion("1.1.0.0"));
            feed.FindLatestByName("FubuCore.Interfaces").Version.ShouldEqual(new SemanticVersion("1.1.0.0"));
        }

        [Test]
        public void updates_the_dependencies()
        {
            var feed = new FileSystemNugetFeed(theOutputDir, NugetStability.Anything);
            var fubuFoundation = feed.FindLatestByName("FubuFoundation");

            var dependency = fubuFoundation.Dependencies().Single();
            dependency.Name.ShouldEqual("FubuCore.Interfaces");
            dependency.VersionSpec.MinVersion.ShouldEqual(new SemanticVersion("1.1.0.0"));
        }
    }
}