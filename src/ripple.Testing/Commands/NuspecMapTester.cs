using System;
using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Commands
{
    [TestFixture]
    public class NuspecMapTester
    {
        private SolutionScenario theScenario;
        private Solution theSolution;
        private NuspecMap theMap;

        [SetUp]
        public void SetUp()
        {
            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                {
                    test.Publishes("Something");
                    test.Publishes("SomeProject");
                    test.Publishes("AnotherProject");

                    test.SolutionDependency("FubuCore", "1.0.0.0", UpdateMode.Fixed);
                    test.ProjectDependency("SomeProject", "FubuCore");
                    test.ProjectDependency("AnotherProject", "FubuCore");
                });
            });

            theSolution = theScenario.Find("Test");
            
            theMap = new NuspecMap
            {
                PackageId = "Something",
                PublishedBy = "SomeProject"
            };
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
        }

        [Test]
        public void finds_the_mapped_nuspecs()
        {
            var spec = theMap.ToSpec(theSolution);

            spec.Project.ShouldBeTheSameAs(theSolution.FindProject("SomeProject"));
            spec.Publishes.Name.ShouldEqual("Something");
        }

        [Test]
        public void blows_up_when_a_nuspec_dependency_does_not_exist()
        {
            var map = new NuspecMap
            {
                PackageId = "Something",
                PublishedBy = "Something",
                DependsOn = "SomeProject2"
            };

            Exception<InvalidOperationException>.ShouldBeThrownBy(() => map.ToSpec(theSolution));
        }

        [Test]
        public void maps_the_nuspec_dependencies()
        {
            var map = new NuspecMap
            {
                PackageId = "AnotherProject",
                PublishedBy = "AnotherProject",
                DependsOn = "Something"
            };

            var spec = map.ToSpec(theSolution);
            spec.Dependencies.Single().Name.ShouldEqual("Something");
        }
    }
}