using System.IO;
using FubuCore;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

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
                new NugetDependency("Bottles"),
                new NugetDependency("CommonServiceLocator"),
                new NugetDependency("FubuCore"),
                new NugetDependency("FubuLocalization"),
                new NugetDependency("HtmlTags", "1.0.0"),
                new NugetDependency("StructureMap")
                );
        }


        [Test]
        public void should_read_all_the_published_assemblies()
        {
            theSpec.PublishedAssemblies.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs("FubuMVC.Core", "FubuMVC.StructureMap");
        
            
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