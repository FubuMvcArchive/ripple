using System;
using System.IO;
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
            FeedScenario.Create(scenario =>
            {
                scenario.For(Feed.NuGetV2)
                        .Add("FubuCore", "1.0.0.0")
                        .Add("FubuCore.Docs", "1.0.0.0")
                        .Add("log4net", "1.0.0.5")
                        .Add("log4net", "1.0.1.1");

                scenario.For(Feed.Fubu)
                    .Add("FubuCore", "1.0.0.100")
                    .Add("FubuCore.Docs", "1.0.0.100")
                    .Add("log4net", "1.0.0.5")
                    .Add("log4net", "1.0.1.1");
            });

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
            FeedRegistry.Reset();
        }

        [Test]
        public void query()
        {
            var sw = new StringWriter();
            var consoleOut = Console.Out;
            Console.SetOut(sw);

            RippleOperation
                .With(theSolution)
                .Execute<FindNugetsInput, FindNugetsCommand>(new FindNugetsInput { Nuget = "FubuCore" });

            var output = sw.ToString();
            consoleOut.Write(output);

            Assert.IsTrue(
                output.Contains(string.Format("FubuCore, 1.0.0.0 ({0})", Feed.NuGetV2.Url)));
            Assert.IsTrue(
                output.Contains(string.Format("FubuCore.Docs, 1.0.0.0 ({0})", Feed.NuGetV2.Url)));
            Assert.IsTrue(
                output.Contains(string.Format("FubuCore, 1.0.0.100 ({0})", Feed.Fubu.Url)));
            Assert.IsTrue(
                output.Contains(string.Format("FubuCore.Docs, 1.0.0.100 ({0})", Feed.Fubu.Url)));
        }
    }
}