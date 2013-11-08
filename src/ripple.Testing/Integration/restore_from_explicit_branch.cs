using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class restore_from_explicit_branch
    {
        private SolutionScenario theScenario;
        private Solution theSolution;
        private Feed theFeedDef;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                {
                    test.SolutionDependency("TestNuGet", "1.1.0.1", UpdateMode.Fixed);
                    test.ProjectDependency("Test", "TestNuGet");
                });
            });

            theFeedDef = new Feed("file://C:/nugets/{branch}");

            theSolution = theScenario.Find("Test");
            theSolution.AddFeed(theFeedDef);

            FeedScenario.Create(scenario =>
            {
                scenario.For("file://C:/nugets/develop")
                        .Add("TestNuGet", "1.1.0.1");
            });

            RippleOperation
                .With(theSolution)
                .Execute<RestoreInput, RestoreCommand>(x => x.BranchFlag = "develop");
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void restores_the_package()
        {
            var local = theSolution.LocalDependencies();
            local.Get("TestNuGet").Version.ShouldEqual(new SemanticVersion("1.1.0.1"));
        }
    }
}