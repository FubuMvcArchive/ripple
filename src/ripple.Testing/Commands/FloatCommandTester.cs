using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Commands
{
    [TestFixture]
    public class FloatCommandTester
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
                });
            });

            theSolution = theScenario.Find("Bottles");

            RippleOperation
                .With(theSolution)
                .Execute<FloatInput, FloatCommand>(new FloatInput { Name = "FubuCore" });

            theSolution = SolutionBuilder.ReadFrom(theScenario.DirectoryForSolution("Bottles"));
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
        }

        [Test]
        public void marks_the_dependency_as_float()
        {
            theSolution.Dependencies.Find("FubuCore").IsFloat().ShouldBeTrue();
        }
    }
}