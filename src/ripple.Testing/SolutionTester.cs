using NUnit.Framework;
using FubuCore;
using FubuTestingSupport;
using ripple.Local;
using ripple.New.Model;
using ripple.Testing.New;

namespace ripple.Testing
{
    [TestFixture]
    public class SolutionTester
    {
        [Test]
        public void create_process_info_for_full_build()
        {
            var solution = new Solution
            {
                SourceFolder = "src",
                BuildCommand = RippleFileSystem.RakeRunnerFile(),
                FastBuildCommand = "rake compile",
				Directory = "directory1".ToFullPath()
            };

            var processInfo = solution.CreateBuildProcess(false);

            processInfo.WorkingDirectory.ShouldEqual("directory1".ToFullPath());
            processInfo.FileName.ShouldEqual(RippleFileSystem.RakeRunnerFile());
            processInfo.Arguments.ShouldBeEmpty();
        }

        [Test]
        public void create_process_for_fast_build()
        {
			var solution = new Solution
			{
				SourceFolder = "src",
				BuildCommand = RippleFileSystem.RakeRunnerFile(),
				FastBuildCommand = "rake compile",
				Directory = "directory1".ToFullPath()
			};

            var processInfo = solution.CreateBuildProcess(true);

            processInfo.WorkingDirectory.ShouldEqual("directory1".ToFullPath());
            processInfo.FileName.ShouldEqual(RippleFileSystem.RakeRunnerFile());
            processInfo.Arguments.ShouldEqual("compile");
        }

        [Test]
        public void get_nuget_directory()
        {
            var solution = new Solution
            {
                SourceFolder = "source",
				Directory = ".".ToFullPath()
            };

	        var storage = new StubNugetStorage();
	        storage.Add("FubuCore", "0.9.1.37");
			solution.UseStorage(storage);

            var project = new Project("something.csproj");
            var dependency = new Dependency("FubuCore", "0.9.1.37");
            project.AddDependency(dependency);
            solution.AddProject(project);

            var spec = new NugetSpec("FubuCore", "somefile.nuspec");

            solution.NugetFolderFor(spec)
                .ShouldEqual(".".ToFullPath().AppendPath(solution.PackagesDirectory(), "FubuCore"));
        
        }
    }
}