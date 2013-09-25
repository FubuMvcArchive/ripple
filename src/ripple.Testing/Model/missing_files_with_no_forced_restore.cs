using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class missing_files_with_no_forced_restore
    {
        private SolutionScenario theScenario;
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Bottles", bottles =>
                {
                    bottles.LocalDependency("FubuCore", "1.0.0.0");
                    bottles.ProjectDependency("Bottles", "FubuCore");
                });
            });

            theSolution = theScenario.Find("Bottles");
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
        }

        [Test]
        public void verify_no_missing_files()
        {
            theSolution.MissingNugets().Any().ShouldBeFalse();
        }
    }
}