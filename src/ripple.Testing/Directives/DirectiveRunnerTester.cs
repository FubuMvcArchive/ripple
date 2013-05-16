using System;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Directives;
using Rhino.Mocks;
using ripple.Model;

namespace ripple.Testing.Directives
{
    [TestFixture]
    public class DirectiveRunnerTester : InteractionContext<DirectiveRunner>
    {
        private string theNugetFolder;

        protected override void beforeEach()
        {
            ClassUnderTest.SetCurrentDirectory("current", "tools");
        }

        [Test]
        public void copy_a_file_with_only_the_nuget()
        {
            theNugetFolder = "nuget-folder";

            MockFor<ISolution>().Stub(x => x.NugetFolderFor("nuget1")).Return(theNugetFolder);

            ClassUnderTest.Copy("assembly.dll", null, "nuget1");

            MockFor<IFileSystem>().AssertWasCalled(x => x.Copy("current".AppendPath("assembly.dll"), theNugetFolder.AppendPath("tools")));
        }

        [Test]
        public void copy_a_file_with_both_nuget_and_location()
        {
            theNugetFolder = "nuget-folder";

            MockFor<ISolution>().Stub(x => x.NugetFolderFor("nuget1")).Return(theNugetFolder);

            ClassUnderTest.Copy("assembly.dll", "lib", "nuget1");

            MockFor<IFileSystem>().AssertWasCalled(x => x.Copy("current".AppendPath("assembly.dll"), theNugetFolder.AppendPath("lib"))); 
        }

        [Test]
        public void copy_a_file_with_only_the_location()
        {
            var solutionDir = "solution";
            MockFor<ISolution>().Stub(x => x.Directory).Return(solutionDir);

            ClassUnderTest.Copy("assembly.dll", "folder1", null);

            MockFor<IFileSystem>().AssertWasCalled(x => x.Copy("current".AppendPath("assembly.dll"), "solution".AppendPath("folder1")));
        }

        [Test]
        public void copy_a_file_with_no_location_or_nuget_goes_to_the_solution_source()
        {
            var solutionDir = "solution";
            MockFor<ISolution>().Stub(x => x.Directory).Return(solutionDir);

            ClassUnderTest.Copy("assembly.dll", null, null);

            MockFor<IFileSystem>().AssertWasCalled(x => x.Copy("current".AppendPath("assembly.dll"), "solution"));
        }
    }

    [TestFixture]
    public class when_creating_a_runner : InteractionContext<DirectiveRunner>
    {
        private string theSolutionDir;

        protected override void beforeEach()
        {
            theSolutionDir = "solution";
            MockFor<ISolution>().Stub(x => x.Directory).Return(theSolutionDir);

            ClassUnderTest.SetCurrentDirectory("current", "tools");

            ClassUnderTest.CreateRunner("BottleRunner.exe", "bottles");
        }

        [Test]
        public void should_have_written_a_file_to_the_root_of_the_project()
        {
	        if (Platform.IsUnix()) return;

            var cmdText = FileSystem.Combine("current", "BottleRunner.exe") + " %*";
            var filename = FileSystem.Combine(theSolutionDir, "bottles.cmd");
            MockFor<IFileSystem>().AssertWasCalled(x => x.WriteStringToFile(filename, cmdText));
        }

        [Test]
        public void should_add_the_file_to_git_ignore()
        {
            if (Platform.IsUnix()) return;
            MockFor<ISolution>().AssertWasCalled(x => x.IgnoreFile("bottles.cmd"));
        }

    }
}