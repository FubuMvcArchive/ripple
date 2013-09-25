using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class remove_a_dependency_from_solution
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
                    test.SolutionDependency("FubuCore", "1.2.0.0", UpdateMode.Float);
                    test.SolutionDependency("FubuLocalization", "1.1.0.0", UpdateMode.Float);
                    
                    test.ProjectDependency("Test", "FubuCore");
                    test.ProjectDependency("Test", "FubuLocalization");
                    
                    test.LocalDependency("FubuCore", "1.2.0.0");
                    test.LocalDependency("FubuLocalization", "1.2.0.0");
                });
            });

            theSolution = theScenario.Find("Test");

            RippleOperation
                .With(theSolution)
                .Execute<RemoveInput, RemoveCommand>(new RemoveInput { Nuget = "FubuCore"});

            theSolution = SolutionBuilder.ReadFrom(theScenario.DirectoryForSolution("Test"));
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
        }

        [Test]
        public void removes_the_solution_level_dependency()
        {
            theSolution.Dependencies.Has("FubuCore").ShouldBeFalse();
        }

        [Test]
        public void removes_the_project_level_dependency()
        {
            theSolution.FindProject("Test").Dependencies.Has("FubuCore").ShouldBeFalse();
        }

        [Test]
        public void removes_the_directory()
        {
            theSolution.LocalDependencies().Has("FubuCore").ShouldBeFalse();
        }
    }
}