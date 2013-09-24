using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Model.Conversion;

namespace ripple.Testing.Model.Conversion
{
    [TestFixture]
    public class finalizing_nuget_solution_loading
    {
        private Solution theSolution;
        private Project p1;
        private Project p2;

        [SetUp]
        public void SetUp()
        {
            theSolution = Solution.NuGet("Test");
            p1 = theSolution.AddProject("Project1");
            p2 = theSolution.AddProject("Project2");

            p1.AddDependency(new Dependency("NuGetA", "1.0.0.0", UpdateMode.Fixed));
            p1.AddDependency(new Dependency("NuGetB", "1.1.0.0", UpdateMode.Fixed));
            p1.AddDependency(new Dependency("NuGetC", "1.4.5.0", UpdateMode.Fixed));

            p2.AddDependency(new Dependency("NuGetD", "1.3.0.0", UpdateMode.Fixed));
            p2.AddDependency(new Dependency("NuGetE", "3.3.0.0", UpdateMode.Fixed));
            p2.AddDependency(new Dependency("NuGetF", "2.1.0.0", UpdateMode.Fixed));

            p2.AddDependency(new Dependency("NuGetB", "1.2.0.0", UpdateMode.Fixed)); // Conflict

            new NuGetSolutionLoader().SolutionLoaded(theSolution);
        }

        private void verifySolutionDependency(string name, string version)
        {
            theSolution.FindDependency(name).ShouldHaveTheSameVersionAs(version);
        }

        private void verifyProjectDependency(string projectName, string name)
        {
            var project = theSolution.FindProject(projectName);
            var dependency = project.Dependencies.Find(name);

            dependency.Mode.ShouldEqual(UpdateMode.Float);
            dependency.Version.ShouldBeEmpty();
        }

        [Test]
        public void dependencies_are_fixed_at_solution_level()
        {
            verifySolutionDependency("NuGetA", "1.0.0.0");
            verifySolutionDependency("NuGetB", "1.2.0.0"); // Take the top when there's a conflict
            verifySolutionDependency("NuGetC", "1.4.5.0");
            verifySolutionDependency("NuGetD", "1.3.0.0");
            verifySolutionDependency("NuGetE", "3.3.0.0");
            verifySolutionDependency("NuGetF", "2.1.0.0");
        }

        [Test]
        public void project_dependencies_are_floated()
        {
            verifyProjectDependency("Project1", "NuGetA");
            verifyProjectDependency("Project1", "NuGetB");
            verifyProjectDependency("Project1", "NuGetC");

            verifyProjectDependency("Project2", "NuGetB");
            verifyProjectDependency("Project2", "NuGetD");
            verifyProjectDependency("Project2", "NuGetE");
            verifyProjectDependency("Project2", "NuGetF");
        }
    }
}