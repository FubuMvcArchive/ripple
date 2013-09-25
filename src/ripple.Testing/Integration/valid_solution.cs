using System.Diagnostics;
using FubuCore.Descriptions;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class valid_solution
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
                    bottles.SolutionDependency("FubuCore", "1.0.0.0", UpdateMode.Fixed);

                    bottles.LocalDependency("FubuCore", "1.0.0.0");
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
        public void the_solution_is_valid()
        {
            var result = theSolution.Validate();
            Debug.WriteLine(result.ToDescriptionText());
            result.IsValid().ShouldBeTrue();
        }
    }

    [TestFixture]
    public class invalid_solution
    {
        private SolutionScenario theScenario;
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", bottles =>
                {
                    bottles.SolutionDependency("FubuCore", "1.1.0.0", UpdateMode.Fixed);
                    bottles.SolutionDependency("Bottles", "1.2.0.0", UpdateMode.Fixed);

                    bottles.LocalDependency("FubuCore", "1.0.0.0");
                    bottles.LocalDependency("Bottles", "1.0.0.0");
                });
            });

            theSolution = theScenario.Find("Test");
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
        }

        [Test]
        public void verify_the_results()
        {
            var results = theSolution.Validate();
            Debug.WriteLine(results.ToDescriptionText());
            results.Problems.ShouldContain(x => x.Provenance == "FubuCore");
            results.Problems.ShouldContain(x => x.Provenance == "Bottles");
        }
    }
}