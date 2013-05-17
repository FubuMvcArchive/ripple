using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Commands
{
    [TestFixture]
    public class LocalNugetInputTester
    {
        private SolutionGraphScenario theScenario;
        private Solution theSolution;
        private LocalNugetInput theInput;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionGraphScenario.Create(scenario =>
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
            theInput = new LocalNugetInput
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
        public void finds_nuspecs_with_corresponding_projects()
        {
            var group = theInput.SpecsFor(theSolution).Single();

            group.Spec.Name.ShouldEqual("SomeProject");
            group.Projects.Single().ShouldBeTheSameAs(theSolution.FindProject("SomeProject"));
        }

        [Test]
        public void no_nuspecs_when_update_dependencies_flag_is_false()
        {
            theInput.UpdateDependenciesFlag = false;
            theInput.SpecsFor(theSolution).Any().ShouldBeFalse();
        }
    }
}