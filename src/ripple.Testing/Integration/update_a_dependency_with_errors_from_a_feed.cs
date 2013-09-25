using System;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class update_a_dependency_with_errors_from_a_feed
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
                    test.SolutionDependency("FubuTransportation", "0.9.0.0", UpdateMode.Float);
                    test.LocalDependency("FubuTransportation", "0.9.0.0");
                });
            });

            theSolution = theScenario.Find("Test");

            FeedScenario.Create(scenario =>
            {
                scenario
                    .For(Feed.Fubu)
                    .Add("FubuTransportation", "0.9.0.1")
                    .ThrowWhenSearchingFor("FubuTransportation", "0.9.0.1", new InvalidOperationException("DNS error?"));

                scenario
                    .For(Feed.NuGetV2)
                    .Add("FubuTransportation", "0.9.0.1");
            });
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void updates_dependency()
        {
            RippleOperation
                .With(theSolution)
                .Execute<UpdateInput, UpdateCommand>(update => update.NugetFlag = "FubuTransportation");

            theSolution.Dependencies.Find("FubuTransportation").Version.ShouldEqual("0.9.0.1");
        }
    }

    [TestFixture]
    public class walk_dependency_tree_with_errors_in_a_feed
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
                    test.SolutionDependency("FubuTransportation", "0.9.0.0", UpdateMode.Float);
                    test.LocalDependency("FubuTransportation", "0.9.0.0");
                });
            });

            theSolution = theScenario.Find("Test");

            FeedScenario.Create(scenario =>
            {
                scenario
                    .For(Feed.Fubu)
                    .Add("FubuTransportation", "0.9.0.1")
                    .ThrowWhenSearchingFor("FubuTransportation", "0.9.0.1", new InvalidOperationException("DNS error?"))
                    .ConfigureRepository(fubu => fubu.ConfigurePackage("FubuTransportation", "0.9.0.1", x => x.DependsOn("FubuCore")));

                scenario
                    .For(Feed.NuGetV2)
                    .Add("FubuTransportation", "0.9.0.1")
                    .Add("FubuCore", "1.1.0.0")
                    .ConfigureRepository(nuget => nuget.ConfigurePackage("FubuTransportation", "0.9.0.1", x => x.DependsOn("FubuCore")));
            });
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void updates_dependency()
        {
            RippleLog.Verbose(true);

            theSolution
                .FeedService
                .DependenciesFor(new Dependency("FubuTransportation", "0.9.0.1"), UpdateMode.Float)
                .ShouldHaveCount(1);
        }
    }
}