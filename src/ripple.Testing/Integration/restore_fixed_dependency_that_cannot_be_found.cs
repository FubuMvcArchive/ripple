using System;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class restore_fixed_dependency_that_cannot_be_found
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
                    test.SolutionDependency("Bottles", "1.1.0.538", UpdateMode.Fixed);
                });
            });

            theSolution = theScenario.Find("Test");

            FeedScenario.Create(scenario =>
            {
                scenario
                    .For(Feed.Fubu)
                    .Add("Bottles", "1.1.0.537");

                scenario
                    .For(Feed.NuGetV2)
                    .Add("Bottles", "1.1.0.538")
                    .ThrowWhenSearchingFor("Bottles", "1.1.0.538", new InvalidOperationException("Test"));
            });
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void throws_error_and_leaves_config_alone()
        {
            Exception<RippleFatalError>.ShouldBeThrownBy(() =>
            {
                RippleOperation
                    .With(theSolution)
                    .Execute<RestoreInput, RestoreCommand>();
            });

            theSolution = theScenario.Find("Test");

            theSolution.Dependencies.Find("Bottles").Version.ShouldEqual("1.1.0.538");
        }
    }

    [TestFixture]
    public class restore_operation_with_invalid_dependencies
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
                    test.SolutionDependency("Bottles", "", UpdateMode.Fixed);
                });
            });

            theSolution = theScenario.Find("Test");

            FeedScenario.Create(scenario =>
            {
                scenario
                    .For(Feed.Fubu)
                    .Add("Bottles", "1.1.0.537");
            });
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void throws_error_and_leaves_config_alone()
        {
            Exception<RippleFatalError>.ShouldBeThrownBy(() =>
            {
                RippleOperation
                    .With(theSolution)
                    .Execute<RestoreInput, RestoreCommand>();
            });

            theSolution = theScenario.Find("Test");

            theSolution.Dependencies.Find("Bottles").Version.ShouldEqual("");
        }
    }
}