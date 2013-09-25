using System;
using System.IO;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class IntegratedInitCommandTester
    {
        private FileSystem theFileSystem;
        private string theSolutionDir;

        [SetUp]
        public void SetUp()
        {
            theFileSystem = new FileSystem();
            theSolutionDir = Guid.NewGuid().ToString().ToFullPath();

            theFileSystem.CreateDirectory(theSolutionDir);
            theFileSystem.CreateDirectory(theSolutionDir, "src");
            theFileSystem.WriteStringToFile(Path.Combine(theSolutionDir, "src", "Solution.sln"), "");

            createProject("ProjectA");
            createProject("ProjectB");
            createProject("ProjectC");

            RippleFileSystem.StopTraversingAt(theSolutionDir.ParentDirectory());
            RippleFileSystem.StubCurrentDirectory(theSolutionDir);

            new InitCommand().Execute(new InitInput { Name = "Test" });
        }

        [TearDown]
        public void TearDown()
        {
            theFileSystem.ForceClean(theSolutionDir);
            theFileSystem.DeleteDirectory(theSolutionDir);

            RippleFileSystem.Live();
        }

        private void createProject(string name)
        {
            var projectDir = theSolutionDir.AppendPath("src", name);
            ProjectGenerator.Create(projectDir, name);
        }

        [Test]
        public void can_read_the_solution()
        {
            var solution = SolutionBuilder.ReadFrom(theSolutionDir);

            solution.FindProject("ProjectA").ShouldNotBeNull();
            solution.FindProject("ProjectB").ShouldNotBeNull();
            solution.FindProject("ProjectC").ShouldNotBeNull();
        }
    }
}