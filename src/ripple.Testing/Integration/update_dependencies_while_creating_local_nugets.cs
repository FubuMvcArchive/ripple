using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;
using ripple.Nuget;
using ripple.Packaging;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class update_dependencies_while_creating_local_nugets
    {
        private SolutionScenario theScenario;
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                {
                    test.Publishes("Something");
                    test.Publishes("SomeProject");

                    test.SolutionDependency("Bottles", "1.1.0.0", UpdateMode.Fixed);
                    test.SolutionDependency("FubuCore", "1.0.0.0", UpdateMode.Float);
                    test.SolutionDependency("FubuLocalization", "1.8.0.0", UpdateMode.Fixed);

                    test.LocalDependency("Bottles", "1.1.0.255");
                    test.LocalDependency("FubuCore", "1.0.1.244");
                    test.LocalDependency("FubuLocalization", "1.8.0.0");

                    test.ProjectDependency("SomeProject", "Bottles");
                    test.ProjectDependency("SomeProject", "FubuCore");
                    test.ProjectDependency("SomeProject", "FubuLocalization");

                    test.ProjectDependency("JustToBeComplicated", "FubuMVC.Core");
                });
            });

            theSolution = theScenario.Find("Test");

            // Map Something.nuspec to the "JustToBeComplicated" project
            theSolution.Nuspecs.Add(new NuspecMap { File = "Something.nuspec", Project = "JustToBeComplicated" });

            var someProject = theSolution.FindProject("SomeProject");
            var justToBeComplicated = theSolution.FindProject("JustToBeComplicated");

            someProject.AddProjectReference(justToBeComplicated);

            theSolution.FindDependency("FubuLocalization").Constraint = "Current,NextMinor";

            RippleOperation
                .With(theSolution, false)
                .Execute<CreatePackagesInput, CreatePackagesCommand>(input =>
                {
                    input.VersionFlag = "1.0.1.244";
                    input.UpdateDependenciesFlag = true;
                });
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
        }

        [Test]
        public void verify_the_dependencies()
        {
            var specFile = theSolution.Specifications.Single(x => x.Name == "SomeProject");
            var spec = NugetSpec.ReadFrom(specFile.Filename);

            verifyVersion(spec, "Bottles", "[1.1.0.255, 2.0.0.0)");
            verifyVersion(spec, "FubuCore", "1.0.1.244");
            verifyVersion(spec, "FubuLocalization", "[1.8.0.0, 1.9.0.0)");
            verifyVersion(spec, "Something", "1.0.1.244");
        }

        private void verifyVersion(NugetSpec spec, string name, string version)
        {
            spec.FindDependency(name).VersionSpec.ToString().ShouldEqual(version);
        }
    }
}