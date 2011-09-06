using NUnit.Framework;
using Rhino.Mocks;
using FubuCore;
using FubuTestingSupport;

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
}