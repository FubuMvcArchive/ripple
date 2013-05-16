using FubuTestingSupport;
using NUnit.Framework;
using ripple.Local;
using ripple.Model;

namespace ripple.Testing.Nuget
{
    [TestFixture]
    public class ProjectNuspecTester
    {
        private Solution theSolution;
        private Project theProject;
        private NugetSpec theNugetSpec;
        private ProjectNuspec theProjectSpec;

        [SetUp]
        public void SetUp()
        {
            theSolution = new Solution();
            theProject = theSolution.AddProject("MyProject");

            theProject.AddDependency("Bottles");
            theProject.AddDependency("FubuCore");
            theProject.AddDependency("FubuLocalization");

            theSolution.AddDependency(new Dependency("Bottles", "1.0.0.0", UpdateMode.Fixed));
            theSolution.AddDependency(new Dependency("FubuCore", "1.1.0.0", UpdateMode.Float));
            theSolution.AddDependency(new Dependency("FubuLocalization", "1.2.0.0", UpdateMode.Fixed));

            theNugetSpec = new NugetSpec("MyProject", "myproject.nuspec");
            // explicit dependencies are not overridden
            theNugetSpec.Dependencies.Add(new NuspecDependency("Bottles", "1.0.0.0"));

            theProjectSpec = new ProjectNuspec(theProject, theNugetSpec);
        }

        [Test]
        public void determines_dependencies_to_generate()
        {
            var dependencies = theProjectSpec.DetermineDependencies();
            dependencies.ShouldHaveTheSameElementsAs(
                theSolution.FindDependency("FubuCore"),
                theSolution.FindDependency("FubuLocalization")
            );
        }
    }
}