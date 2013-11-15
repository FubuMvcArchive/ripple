using System.Collections.Generic;
using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;
using ripple.Packaging;

namespace ripple.Testing.Packaging
{
    [TestFixture]
    public class DirectDependenciesSourceIntregatedTester
    {
        private SolutionScenario theScenario;
        private Solution theSolution;
        private NuspecTemplateContext theContext;
        private IEnumerable<NuspecDependencyToken> theTokens;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                {
                    test.Publishes("MyProject");

                    test.SolutionDependency("Bottles", "1.1.0.0", UpdateMode.Fixed);
                    test.SolutionDependency("FubuCore", "1.0.0.0", UpdateMode.Float);
                    test.SolutionDependency("FubuLocalization", "1.8.0.0", UpdateMode.Fixed);

                    test.LocalDependency("Bottles", "1.1.0.255");
                    test.LocalDependency("FubuCore", "1.0.1.244");
                    test.LocalDependency("FubuLocalization", "1.8.0.0");

                    test.ProjectDependency("MyProject", "Bottles");
                    test.ProjectDependency("MyProject", "FubuCore");
                    test.ProjectDependency("MyProject", "FubuLocalization");
                });
            });

            theSolution = theScenario.Find("Test");

            var templates = new NuspecTemplateFinder().Templates(theSolution);
            var current = templates.Single();

            theContext = new NuspecTemplateContext(current, templates, theSolution);
            theTokens = new DirectDependenciesSource().DetermineDependencies(theContext);
        }

        [Test]
        public void generates_the_bottles_token()
        {
            var bottles = theTokens.Single(x => x.Dependency.MatchesName("Bottles"));
            bottles.Constraint.ShouldEqual(theSolution.NuspecSettings.Fixed);
            bottles.Version.ShouldEqual(new SemanticVersion("1.1.0.255"));
        }

        [Test]
        public void generates_the_fubucore_token()
        {
            var fubuCore = theTokens.Single(x => x.Dependency.MatchesName("FubuCore"));
            fubuCore.Constraint.ShouldEqual(theSolution.NuspecSettings.Float);
            fubuCore.Version.ShouldEqual(new SemanticVersion("1.0.1.244"));
        }

        [Test]
        public void generates_the_fubulocalization_token()
        {
            var fubuLocalization = theTokens.Single(x => x.Dependency.MatchesName("FubuLocalization"));
            fubuLocalization.Constraint.ShouldEqual(theSolution.NuspecSettings.Fixed);
            fubuLocalization.Version.ShouldEqual(new SemanticVersion("1.8.0.0"));
        }
    }
}