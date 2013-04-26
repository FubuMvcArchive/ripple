using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class missing_files_with_force_restore_of_specific_dependency
    {
        private SolutionGraphScenario theScenario;
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionGraphScenario.Create(scenario =>
            {
                scenario.Solution("Bottles", bottles =>
                {
                    bottles.LocalDependency("FubuCore", "1.0.0.0");
                    bottles.LocalDependency("FubuLocalization", "1.0.0.0");

                    bottles.ProjectDependency("Bottles", "FubuCore");
                    bottles.ProjectDependency("Bottles", "FubuLocalization");
                });
            });

            theSolution = theScenario.Find("Bottles");
            theSolution.ForceRestore("FubuCore");
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
        }

        [Test]
        public void verify_missing_files()
        {
            theSolution.MissingNugets().ShouldHaveTheSameDependenciesAs("FubuCore");
        }
    }
}