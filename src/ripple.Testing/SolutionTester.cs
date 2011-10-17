using System.Diagnostics;
using NUnit.Framework;
using Rhino.Mocks;
using FubuCore;
using FubuTestingSupport;
using System.Linq;
using System.Collections.Generic;
using ripple.Local;
using ripple.Model;

namespace ripple.Testing
{
    [TestFixture]
    public class SolutionTester
    {

        [Test]
        public void adding_a_project_should_set_the_update_mode_on_all_nuget_dependencies()
        {
            var solution = new Solution(new SolutionConfig()
            {
                SourceFolder = "src",
                BuildCommand = RippleFileSystem.RakeRunnerFile(),
                FastBuildCommand = "rake compile",
                Floats = new string[]{"Nug1", "Nug2"},
            }, "directory1");


            var project = new Project("something.csproj");
            project.AddDependency(new NugetDependency("Nug1"));
            project.AddDependency(new NugetDependency("Nug2"));
            project.AddDependency(new NugetDependency("Nug3"));
            project.AddDependency(new NugetDependency("Nug4"));
        
            solution.AddProject(project);

            project.FindDependency("Nug1").UpdateMode.ShouldEqual(UpdateMode.Float);
            project.FindDependency("Nug2").UpdateMode.ShouldEqual(UpdateMode.Float);
            project.FindDependency("Nug3").UpdateMode.ShouldEqual(UpdateMode.Locked);
            project.FindDependency("Nug4").UpdateMode.ShouldEqual(UpdateMode.Locked);
        }

        [Test]
        public void create_process_info_for_full_build()
        {
            var solution = new Solution(new SolutionConfig()
            {
                SourceFolder = "src",
                BuildCommand = RippleFileSystem.RakeRunnerFile(),
                FastBuildCommand = "rake compile"
            }, "directory1");

            var processInfo = solution.CreateBuildProcess(false);

            processInfo.WorkingDirectory.ShouldEqual("directory1".ToFullPath());
            processInfo.FileName.ShouldEqual(RippleFileSystem.RakeRunnerFile());
            processInfo.Arguments.ShouldBeEmpty();
        }

        [Test]
        public void create_process_for_fast_build()
        {
            var solution = new Solution(new SolutionConfig()
            {
                SourceFolder = "src",
                BuildCommand = RippleFileSystem.RakeRunnerFile(),
                FastBuildCommand = "rake compile"
            }, "directory1");

            var processInfo = solution.CreateBuildProcess(true);

            processInfo.WorkingDirectory.ShouldEqual("directory1".ToFullPath());
            processInfo.FileName.ShouldEqual(RippleFileSystem.RakeRunnerFile());
            processInfo.Arguments.ShouldEqual("compile");
        }


        [Test]
        public void clean_deletes_the_packages_folder_in_all_mode()
        {
            var solution = new Solution(new SolutionConfig(){
                SourceFolder = "src"
            }, "directory1");

            var fileSystem = DataMother.MockedFileSystem();

            solution.Clean(fileSystem, CleanMode.all);

            fileSystem.AssertWasCalled(x => x.DeleteDirectory("directory1".ToFullPath(), "src", "packages"));
        }

        [Test]
        public void clean_deletes_the_packages_folder_in_packages_mode()
        {
            var solution = new Solution(new SolutionConfig()
            {
                SourceFolder = "src"
            }, "directory1");

            var fileSystem = DataMother.MockedFileSystem();

            solution.Clean(fileSystem, CleanMode.packages);

            fileSystem.AssertWasCalled(x => x.DeleteDirectory("directory1".ToFullPath(), "src", "packages"));
        }

        [Test]
        public void clean_does_not_delete_the_packages_folder_in_projects_mode()
        {
            var solution = new Solution(new SolutionConfig()
            {
                SourceFolder = "src"
            }, "directory1");

            var fileSystem = DataMother.MockedFileSystem();

            solution.Clean(fileSystem, CleanMode.projects);

            fileSystem.AssertWasNotCalled(x => x.DeleteDirectory("directory1".ToFullPath(), "src", "packages"));
        }

        [Test]
        public void add_a_nuget_spec_should_add_it_to_the_collection_and_set_itself_as_the_publisher_to_the_spec()
        {
            var spec = new NugetSpec("fubucore", "somefile.nuspec");

            var solution = new Solution(new SolutionConfig()
            {
                SourceFolder = "src"
            }, "directory1");

            solution.AddNugetSpec(spec);

            spec.Publisher.ShouldBeTheSameAs(solution);
            solution.PublishedNugets.ShouldContain(spec);
        }

        [Test]
        public void get_nuget_directory()
        {
            var solution = new Solution(new SolutionConfig()
            {
                SourceFolder = "source"
            }, ".".ToFullPath());

            var project = new Project("something.csproj");
            var dependency = new NugetDependency("FubuCore", "0.9.1.37");
            project.AddDependency(dependency);
            solution.AddProject(project);

            var spec = new NugetSpec("FubuCore", "somefile.nuspec");

            solution.NugetFolderFor(spec)
                .ShouldEqual(".".ToFullPath().AppendPath(solution.Config.SourceFolder, "packages", "FubuCore.0.9.1.37"));
        
        }
    }

    [TestFixture]
    public class when_reading_a_solution_from_the_file_system
    {
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            DataMother.CreateDataFolder();

            theSolution = Solution.ReadFrom("data".AppendPath("fubucore"));
            theSolution.ShouldNotBeNull();
        }

        [Test]
        public void should_have_the_name_from_the_ripple_file()
        {
            theSolution.Name.ShouldEqual("fubucore");
        }

        [Test]
        public void should_load_the_published_nugets()
        {
            theSolution.PublishedNugets
                .Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("FubuCore", "FubuLocalization", "FubuTestingSupport");
        }

        [Test]
        public void should_load_all_the_projects()
        {
            var projectNames = theSolution.Projects.Select(x => x.ProjectName).OrderBy(x => x);

            projectNames.ShouldHaveTheSameElementsAs("FubuCore", "FubuCore.Testing", "FubuLocalization.Tests", "FubuTestingSupport");
        }

        [Test]
        public void a_project_should_have_all_of_its_nuget_dependencies()
        {
            theSolution.FindProject("FubuTestingSupport")
                .NugetDependencies.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("NUnit", "structuremap", "structuremap.automocking", "RhinoMocks", "CommonServiceLocator");
        }
    }


}