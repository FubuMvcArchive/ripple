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
			var theFilename = "fubumvc.core.nuspec";
			var stream = GetType()
					.Assembly
					.GetManifestResourceStream(typeof(DataMother), "FubuMVCNuspecTemplate.txt");

			new FileSystem().WriteStreamToFile(theFilename, stream);
            theSpec = NugetSpec.ReadFrom(theFilename.ToFullPath());
        }

        [Test]
        public void should_read_the_name()
        {
            theSpec.Name.ShouldEqual("FubuMVC.Core");
        }

        [Test]
        public void should_read_all_the_dependencies()
        {
            theSpec.Dependencies.ShouldHaveTheSameElementsAs(
				new Dependency("Bottles", "1.0.0.412"),
				new Dependency("FubuCore", "0.9.9.172"),
				new Dependency("FubuLocalization", "0.9.5.48"),
				new Dependency("HtmlTags", "1.1.0.114")
            );
        }


        [Test]
        public void should_read_all_the_published_assemblies()
        {
            var names = theSpec.PublishedAssemblies.Select(x => x.Name);
            names.ShouldContain("FubuMVC.Core");

        }

        [Test]
        public void read_the_published_assembly_correctly()
        {
            var assembly = theSpec.PublishedAssemblies.First(x => x.Name == "FubuMVC.Core");

            var expectedPath = Path.GetDirectoryName(theSpec.Filename)
                .AppendPath("..", "..", "src", "FubuMVC.Core", "bin", "release")
                .ToFullPath();

            assembly.Directory.ShouldEqual(expectedPath);
            assembly.Pattern.ShouldEqual("FubuMVC.Core.*");
        }
    }
}