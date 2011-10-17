using NUnit.Framework;
using ripple.Commands;
using ripple.Local;
using ripple.Model;
using FubuTestingSupport;

namespace ripple.Testing.Commands
{
    [TestFixture]
    public class UpdateInputTester
    {
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            theSolution = new Solution(new SolutionConfig()
                                       {
                                           SourceFolder = "src",
                                           BuildCommand = RippleFileSystem.RakeRunnerFile(),
                                           FastBuildCommand = "rake compile",
                                           Floats = new string[] { "Nug1", "Nug2" },
                                       }, "directory1");


            var project1 = new Project("something.csproj");
            project1.AddDependency(new NugetDependency("Nug1"));
            project1.AddDependency(new NugetDependency("Nug2"));
            project1.AddDependency(new NugetDependency("Nug3"));
            project1.AddDependency(new NugetDependency("Nug4"));

            theSolution.AddProject(project1);

            var project2 = new Project("something.csproj");
            project2.AddDependency(new NugetDependency("Nug1"));
            project2.AddDependency(new NugetDependency("Nug3"));
            project2.AddDependency(new NugetDependency("Nug5"));
            project2.AddDependency(new NugetDependency("Nug6"));

            theSolution.AddProject(project2);
        }

        [Test]
        public void get_the_list_of_nugets_to_update_with_the_nuget_flag()
        {
            var input = new UpdateInput{
                NugetFlag = "Nug1"
            };

            input.GetAllNugetNames(theSolution).ShouldHaveTheSameElementsAs("Nug1");
        }

        [Test]
        public void get_the_list_of_nugets_returns_the_specified_nuget_even_if_it_is_locked()
        {
            var input = new UpdateInput
            {
                NugetFlag = "Nug6"
            };

            input.GetAllNugetNames(theSolution).ShouldHaveTheSameElementsAs("Nug6");
        }

        [Test]
        public void normal_operation_of_get_names_returns_all_floats()
        {
            new UpdateInput().GetAllNugetNames(theSolution)
                .ShouldHaveTheSameElementsAs("Nug1", "Nug2");
        }

        [Test]
        public void using_the_force_flag_returns_all_nuget_names()
        {
            new UpdateInput{ForceFlag = true}.GetAllNugetNames(theSolution)
                .ShouldHaveTheSameElementsAs("Nug1", "Nug2", "Nug3", "Nug4", "Nug5", "Nug6");
        }
    }
}