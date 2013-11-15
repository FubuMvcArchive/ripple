using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Packaging;

namespace ripple.Testing.Packaging
{
    [TestFixture]
    public class finding_nuspecs_by_convention_and_mapping
    {
        private SolutionScenario theScenario;
        private Solution theSolution;
        private NuspecTemplateFinder theFinder;

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
            theSolution.AddNuspec(new NuspecMap { PackageId = "SomethingElse", PublishedBy = "Something" });

            theFinder = new NuspecTemplateFinder();
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
        }

        [Test]
        public void finds_nuspecs_with_corresponding_and_mapped_projects()
        {
            var groups = theFinder.Templates(theSolution).ToArray();

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