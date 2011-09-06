using System.Diagnostics;
using NUnit.Framework;
using Rhino.Mocks;
using FubuCore;
using FubuTestingSupport;
using System.Linq;
using System.Collections.Generic;

namespace ripple.Testing
{
    [TestFixture]
    public class SolutionTester
    {
        [Test]
        public void clean_deletes_the_packages_folder()
        {
            var solution = new Solution(new SolutionConfig(){
                SourceFolder = "src"
            }, "directory1");

            var fileSystem = DataMother.MockedFileSystem();

            solution.Clean(fileSystem);

            fileSystem.AssertWasCalled(x => x.DeleteDirectory("directory1", "src", "packages"));
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