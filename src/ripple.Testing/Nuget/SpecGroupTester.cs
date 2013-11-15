using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;
using ripple.Packaging;

namespace ripple.Testing.Nuget
{
    [TestFixture]
    public class SpecGroupTester
    {
        private Solution theSolution;
        private Project p1;
        private Project p2;
        private NugetSpec theNugetSpec;
        private NuspecTemplate theGroup;

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

            theGroup = new NuspecTemplate(theNugetSpec, new[]
            {
                new ProjectNuspec(p1, new NugetSpec("MyProject", "MyProject.nuspec")), 
                new ProjectNuspec(p2, new NugetSpec("MyOtherProject", "MyOtherProject.nuspec"))
            });
        }

        [Test]
        public void determines_dependencies_to_generate()
        {
            var dependencies = theGroup.DetermineDependencies();
            dependencies.ShouldHaveTheSameElementsAs(
                theSolution.FindDependency("Bottles"),
                theSolution.FindDependency("FubuCore"),
                theSolution.FindDependency("FubuLocalization"),
                theSolution.FindDependency("FubuMVC.Core")
            );
        }
    }
}