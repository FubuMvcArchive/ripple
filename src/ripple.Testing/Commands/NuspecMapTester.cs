using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Commands
{
    [TestFixture]
    public class NuspecMapTester
    {
        private SolutionScenario theScenario;
        private Solution theSolution;
        private NuspecMap theMap;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionScenario.Create(scenario =>
                {
                    scenario.Solution("Test", test =>
                        {
                            test.Publishes("Something");
                            test.Publishes("SomeProject");

                            test.SolutionDependency("FubuCore", "1.0.0.0", UpdateMode.Fixed);
                            test.ProjectDependency("SomeProject", "FubuCore");
                        });
                });

            theSolution = theScenario.Find("Test");
            
            theMap = new NuspecMap
                {
                    File = "Something.nuspec",
                    Project = "SomeProject"
                };
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
        }

        [Test]
        public void finds_the_mapped_nuspecs()
        {
            var spec = theMap.ToSpec(theSolution);

            spec.Project.ShouldBeTheSameAs(theSolution.FindProject("SomeProject"));
            spec.Spec.Name.ShouldEqual("Something");
        }
    }
}