using System.IO;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing
{
    [TestFixture]
    public class RippleFileSystemTester
    {
        private string theSolutionDir;
        private string theCurrentDir;
        private string theCodeDir;

        [SetUp]
        public void SetUp()
        {
            theCodeDir = ".".AppendPath("code");
            theSolutionDir = theCodeDir.AppendPath("ripple");
            theCurrentDir = theSolutionDir.AppendPath("src", "project1");

            var fileSystem = new FileSystem();
            fileSystem.CreateDirectory(theCodeDir);
            fileSystem.CreateDirectory(theSolutionDir);
            fileSystem.CreateDirectory(theCurrentDir);

            fileSystem.WriteStringToFile(Path.Combine(theSolutionDir, SolutionFiles.ConfigFile), "");

            RippleFileSystem.StubCurrentDirectory(theCurrentDir);
        }

        [TearDown]
        public void TearDown()
        {
            var fileSystem = new FileSystem();
            fileSystem.DeleteDirectory(theCodeDir);
        }

        [Test]
        public void finds_the_solution_dir()
        {
            RippleFileSystem.FindSolutionDirectory().ShouldEqual(theSolutionDir.ToFullPath());
        }

        [Test]
        public void finds_the_code_dir()
        {
            RippleFileSystem.FindCodeDirectory().ShouldEqual(theCodeDir.ToFullPath());
        }
    }
}