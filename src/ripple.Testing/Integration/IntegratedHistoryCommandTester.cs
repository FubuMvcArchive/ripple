using System;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class IntegratedHistoryCommandTester
    {
        private SolutionScenario theScenario;
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                {
                    test.SolutionDependency("FubuCore", "1.0.0.0", UpdateMode.Float);
                    test.SolutionDependency("structuremap", "2.6.3", UpdateMode.Fixed);
                    
                    test.LocalDependency("FubuCore", "1.0.0.0");
                    test.LocalDependency("structuremap", "2.6.3");
                });
            });

            theSolution = theScenario.Find("Test");

            RippleOperation
                .With(theSolution)
                .Execute<HistoryInput, HistoryCommand>();
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
        }

        [Test]
        public void verify_the_history()
        {
            var file = theSolution.Directory.AppendPath("artifacts", "dependency-history.txt");
            var history = new FileSystem().ReadStringFromFile(file);
            var lines = history.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

            lines[0].ShouldEqual("FubuCore/1.0.0.0");
            lines[1].ShouldEqual("structuremap/2.6.3");
        }
    }
}