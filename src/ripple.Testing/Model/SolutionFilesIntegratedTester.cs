using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Model.Conversion;
using ripple.Model.Xml;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class SolutionFilesIntegratedTester
    {
        [Test]
        public void files_for_nuget_solution()
        {
            using (var sandbox = DirectorySandbox.Create())
            {
                sandbox.CreateFile("MySolution.sln");
                sandbox.CreateDirectory("MyProject");
                sandbox.CreateFile("MyProject", "MyProject.csproj");
                sandbox.CreateFile("MyProject", "packages.config");

                SolutionFiles
                    .FromDirectory(sandbox.Directory)
                    .Loader
                    .ShouldBeOfType<NuGetSolutionLoader>();
            }
        }

        [Test]
        public void files_for_empty_ripple_solution()
        {
            using (var sandbox = DirectorySandbox.Create())
            {
                sandbox.StopAtParent();

                // TODO -- Going to need to write out some XML to compare the ObjectBlocks vs. XML loaders
                sandbox.CreateFile("ripple.config");

                sandbox.CreateDirectory("src");
                sandbox.CreateFile("src", "MySolution.sln");

                sandbox.CreateDirectory("src", "MyProject");
                sandbox.CreateFile("src", "MyProject", "MyProject.csproj");

                SolutionFiles
                    .FromDirectory(sandbox.Directory)
                    .Loader
                    .ShouldBeOfType<XmlSolutionLoader>();
            }
        }
    }
}