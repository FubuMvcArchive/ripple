using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class installing_an_existing_project_dependency
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
                        test.ProjectDependency("Test", "FubuCore");
                    });
            });

            theSolution = theScenario.Find("Test");
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void throws_ripple_assertion_for_existing_dependency()
        {
            Exception<RippleFatalError>.ShouldBeThrownBy(() =>
            {
                RippleOperation
                    .With(theSolution)
                    .Execute<InstallInput, InstallCommand>(x =>
                        {
                            x.Package = "FubuCore";
                            x.ProjectFlag = "Test";
                        });

            }).Message.ShouldEqual("FubuCore already exists in Project Test");
        }
    }
}