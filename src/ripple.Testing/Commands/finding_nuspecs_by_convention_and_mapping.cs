using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;
using ripple.Packaging;

namespace ripple.Testing.Commands
{
    [TestFixture]
    public class finding_nuspecs_by_convention_and_mapping
    {
        private SolutionScenario theScenario;
        private Solution theSolution;
        private CreatePackagesInput theInput;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                    {
                        test.Publishes("SomethingElse");
                        test.Publishes("SomeProject");

                        test.SolutionDependency("FubuCore", "1.0.0.0", UpdateMode.Fixed);
                        test.ProjectDependency("SomeProject", "FubuCore");
                        test.ProjectDependency("Something", "FubuCore");
                    });
            });

            theSolution = theScenario.Find("Test");
            theSolution.AddNuspec(new NuspecMap { File = "SomethingElse.nuspec", Project = "Something" });

            theInput = new CreatePackagesInput
            {
                UpdateDependenciesFlag = true
            };
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
        }

        [Test]
        public void finds_nuspecs_with_corresponding_and_mapped_projects()
        {
            var groups = theInput.SpecsFor(theSolution).ToArray();

            var spec1 = groups[0].Spec;
            spec1.Name.ShouldEqual("SomeProject");

            var p1 = groups[0].Projects.Single();
            p1.ShouldBeTheSameAs(theSolution.FindProject("SomeProject"));


            var spec2 = groups[1].Spec;
            spec2.Name.ShouldEqual("SomethingElse");

            var p2 = groups[1].Projects.Single();
            p2.ShouldBeTheSameAs(theSolution.FindProject("Something"));
        }
    }
}