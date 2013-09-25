using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Commands
{
    [TestFixture]
    public class floating_command_with_min_version
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
                .Execute<FloatInput, FloatCommand>(new FloatInput { Name = "FubuCore", MinVersionFlag = "1.2.0.0" });

            theSolution = SolutionBuilder.ReadFrom(theScenario.DirectoryForSolution("Bottles"));
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
        }

        [Test]
        public void marks_the_min_version()
        {
            theSolution.Dependencies.Find("FubuCore").Version.ShouldEqual("1.2.0.0");
        }
    }
}