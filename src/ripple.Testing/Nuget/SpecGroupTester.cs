using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget
{
    [TestFixture]
    public class SpecGroupTester
    {
        private Solution theSolution;
        private Project p1;
        private Project p2;
        private NugetSpec theNugetSpec;
        private SpecGroup theGroup;

        [SetUp]
        public void SetUp()
        {
            theSolution = new Solution();
            p1 = theSolution.AddProject("MyProject");

            p1.AddDependency("Bottles");
            p1.AddDependency("FubuCore");
            p1.AddDependency("FubuLocalization");

            p2 = theSolution.AddProject("MyOtherProject");
            p2.AddDependency("FubuMVC.Core");

            theSolution.AddDependency(new Dependency("Bottles", "1.0.0.0", UpdateMode.Fixed));
            theSolution.AddDependency(new Dependency("FubuCore", "1.1.0.0", UpdateMode.Float));
            theSolution.AddDependency(new Dependency("FubuLocalization", "1.2.0.0", UpdateMode.Fixed));
            theSolution.AddDependency(new Dependency("FubuMVC.Core", "1.4.0.0", UpdateMode.Fixed));

            theNugetSpec = new NugetSpec("MyProject", "myproject.nuspec");
            // explicit dependencies are not overridden
            theNugetSpec.Dependencies.Add(new NuspecDependency("Bottles", "1.0.0.0"));

            theGroup = new SpecGroup(theNugetSpec, new[] { p1, p2 });
        }

        [Test]
        public void determines_dependencies_to_generate()
        {
            var dependencies = theGroup.DetermineDependencies();
            dependencies.ShouldHaveTheSameElementsAs(
                theSolution.FindDependency("FubuCore"),
                theSolution.FindDependency("FubuLocalization"),
                theSolution.FindDependency("FubuMVC.Core")
            );
        }
    }
}