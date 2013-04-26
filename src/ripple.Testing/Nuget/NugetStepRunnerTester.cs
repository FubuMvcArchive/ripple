using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget
{
    [TestFixture]
    public class NugetStepRunnerTester
    {
        private Solution theSolution;
        private Project theProject;
        private NugetStepRunner theRunner;

        [SetUp]
        public void SetUp()
        {
            theSolution = new Solution();
            theProject = theSolution.AddProject("Project1");

            theRunner = new NugetStepRunner(theSolution);
        }

        [Test]
        public void solution_level_dependency()
        {
            var dependency = new Dependency("FubuCore", "1.2.0.0", UpdateMode.Fixed);
            theRunner.AddSolutionDependency(dependency);

            theSolution.Dependencies.Find("FubuCore").ShouldEqual(dependency);
        }

        [Test]
        public void project_level_dependency()
        {
            var dependency = new Dependency("FubuCore", "1.2.0.0", UpdateMode.Fixed);
            theRunner.AddProjectDependency("Project1", dependency);

            theProject.Dependencies.Find("FubuCore").ShouldEqual(dependency.AsFloat());
        }

        [Test]
        public void updating_solution_level_dependencies()
        {
            var oldDependency = new Dependency("FubuCore", "1.2.0.0", UpdateMode.Fixed);
            theSolution.AddDependency(oldDependency);

            var newDependency = new Dependency("FubuCore", "1.3.0.0", UpdateMode.Fixed);
            theRunner.UpdateDependency(newDependency);

            theSolution.Dependencies.Find("FubuCore").ShouldEqual(newDependency);
        }

        [Test]
        public void updating_a_solution_level_dependency_forces_a_restore()
        {
            var oldDependency = new Dependency("FubuCore", "1.2.0.0", UpdateMode.Fixed);
            theSolution.AddDependency(oldDependency);

            var newDependency = new Dependency("FubuCore", "1.3.0.0", UpdateMode.Fixed);
            theRunner.UpdateDependency(newDependency);

            theSolution.RestoreSettings.ShouldForce(new Dependency("FubuCore")).ShouldBeTrue();
        }
    }
}