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
    public class ProjectDependenciesSourceIntregatedTester
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
                    test.Publishes("AnotherProject");

                    test.SolutionDependency("Bottles", "1.1.0.0", UpdateMode.Fixed);
                    test.SolutionDependency("FubuCore", "1.0.0.0", UpdateMode.Float);
                    test.SolutionDependency("FubuLocalization", "1.8.0.0", UpdateMode.Fixed);

                    test.LocalDependency("Bottles", "1.1.0.255");
                    test.LocalDependency("FubuCore", "1.0.1.244");
                    test.LocalDependency("FubuLocalization", "1.8.0.0");

                    test.ProjectDependency("MyProject", "Bottles");
                    test.ProjectDependency("MyProject", "FubuCore");
                    test.ProjectDependency("MyProject", "FubuLocalization");

                    test.ProjectDependency("AnotherProject", "FubuCore");
                });
            });

            theSolution = theScenario.Find("Test");
            theSolution.FindProject("AnotherProject").AddProjectReference(theSolution.FindProject("MyProject"));

            var templates = new NuspecTemplateFinder().Templates(theSolution);
            var current = templates.FindByProject("AnotherProject");

            theContext = new NuspecTemplateContext(current, templates, theSolution, new SemanticVersion("1.1.0.0"));
            theTokens = new ProjectDependenciesSource().DetermineDependencies(theContext);
        }

        [Test]
        public void generates_the_project_dependency_token()
        {
            var project = theTokens.Single(x => x.Dependency.MatchesName("MyProject"));
            project.Constraint.ShouldEqual(theSolution.NuspecSettings.Float);
            project.Version.ShouldEqual(theContext.Version);
        }
    }
}