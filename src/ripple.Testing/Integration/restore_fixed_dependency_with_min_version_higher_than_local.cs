using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class restore_fixed_dependency_with_min_version_higher_than_local
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
                    test.SolutionDependency("Dovetail.SDK", "3.3.0.23", UpdateMode.Fixed);
                    test.ProjectDependency("Test", "Dovetail.SDK");
                    test.LocalDependency("Dovetail.SDK", "3.2.10.27");
                });
            });

            theSolution = theScenario.Find("Test");

            FeedScenario.Create(scenario =>
            {
                scenario
                    .For(Feed.Fubu)
                    .Add("Dovetail.SDK", "3.2.10.27")
                    .Add("Dovetail.SDK", "3.3.0.23");
            });

            RippleOperation
                .With(theSolution)
                .Execute<RestoreInput, RestoreCommand>();
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void restores_the_min_version()
        {
            var local = theSolution.LocalDependencies();
            local.Get("Dovetail.SDK").Version.ShouldEqual(new SemanticVersion("3.3.0.23"));
        }
    }
}