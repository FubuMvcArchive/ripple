using NUnit.Framework;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Commands
{
    [TestFixture]
    public class FindNugetsCommandTester
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
                    bottles.SolutionDependency("FubuCore", "1.0.0.0", UpdateMode.Fixed);
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
        public void TesT()
        {
            RippleOperation
                .With(theSolution)
                .Execute<FindNugetsInput, FindNugetsCommand>(new FindNugetsInput {Nuget = "FubuCore"});
        }
    }
}