using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class updating_a_float_dependency_when_a_cached_version_exists
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
                            test.SolutionDependency("FubuTransportation", "0.9.0.1", UpdateMode.Float);
                        });
                });

            theSolution = theScenario.Find("Test");

            FeedScenario.Create(scenario =>
                {
                    scenario
                        .For(Feed.Fubu)
                        .Add("FubuTransportation", "0.9.0.1")
                        .Add("FubuTransportation", "0.9.1.0");

                    scenario
                        .For(theSolution.Cache.ToFeed())
                        .Add("FubuTransportation", "0.9.0.1");
                });

            

            RippleOperation
                .With(theSolution)
                .Execute<UpdateInput, UpdateCommand>(input =>
                    {
                        input.NugetFlag = "FubuTransportation";
                    });
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void updates_to_the_latest()
        {
            theSolution.Dependencies.Find("FubuTransportation").Version.ShouldEqual("0.9.1.0");
        }
    }
}