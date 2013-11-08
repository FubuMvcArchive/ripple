using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
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