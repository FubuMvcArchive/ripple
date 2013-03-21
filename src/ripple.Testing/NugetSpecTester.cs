using System.IO;
using FubuCore;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;
using ripple.Local;
using ripple.New.Model;

namespace ripple.Testing
{
    [TestFixture]
    public class NugetSpecTester
    {
        private NugetSpec theSpec;

        [SetUp]
        public void SetUp()
        {
            var file = FileSystem.Combine("..", "..", "data", "fubumvc","packaging", "nuget", "fubumvc.references.nuspec");
            theSpec = NugetSpec.ReadFrom(file.ToFullPath());
        }

        [Test]
        public void should_read_the_name()
        {
            theSpec.Name.ShouldEqual("FubuMVC.References");
        }

        [Test]
        public void should_read_all_the_dependencies()
        {
            theSpec.Dependencies.ShouldHaveTheSameElementsAs(
                new Dependency("Bottles"),
				new Dependency("CommonServiceLocator"),
				new Dependency("FubuCore"),
				new Dependency("FubuLocalization"),
				new Dependency("HtmlTags", "1.0.0"),
				new Dependency("StructureMap")
            );
        }


        [Test]
        public void should_read_all_the_published_assemblies()
        {
            var names = theSpec.PublishedAssemblies.Select(x => x.Name);
            names.ShouldContain("FubuMVC.Core");
            names.ShouldContain("FubuMVC.StructureMap");

        }

        [Test]
        public void read_the_published_assembly_correctly()
        {
            var assembly = theSpec.PublishedAssemblies.First(x => x.Name == "FubuMVC.Core");

            var expectedPath = Path.GetDirectoryName(theSpec.Filename)
                .AppendPath("..", "..", "build")
                .ToFullPath();

            assembly.Directory.ShouldEqual(expectedPath);
            assembly.Pattern.ShouldEqual("FubuMVC.Core.*");
        }
    }
}