using FubuCore;
using NUnit.Framework;
using NuGet;
using FubuTestingSupport;
using System.Linq;
using ripple.New.Nuget;

namespace ripple.Testing.New.Nuget
{
    [TestFixture]
    public class NugetFileTester
    {
		private NugetFile fileFor(string path)
		{
			new FileSystem().WriteStringToFile(path, "test");
			return new NugetFile(path);
		}

		[Test]
		public void parsing_smoke()
		{
			var file = fileFor("Newtonsoft.Json.4.5.11.nupkg");
			file.Name.ShouldEqual("Newtonsoft.Json");
			file.Version.ShouldEqual(SemanticVersion.Parse("4.5.11"));
			file.IsPreRelease.ShouldBeFalse();
		}

		[Test]
		public void file_with_number_in_name()
		{
			var file = fileFor("Storyteller2.2.0.0.15.nupkg");
			file.Name.ShouldEqual("Storyteller2");
			file.Version.ShouldEqual(SemanticVersion.Parse("2.0.0.15"));
			file.IsPreRelease.ShouldBeFalse();
		}

		[Test]
		public void file_with_number_in_name_and_special_version()
		{
			var file = fileFor("Storyteller2.2.0.0.15-alpha.nupkg");
			file.Name.ShouldEqual("Storyteller2");
			file.Version.ShouldEqual(SemanticVersion.Parse("2.0.0.15-alpha"));
			file.IsPreRelease.ShouldBeTrue();
		}

        [Test]
        public void build_a_nuget_file_by_file_name()
        {
            var file = fileFor("Bottles.1.1.0.441.nupkg");
            file.Name.ShouldEqual("Bottles");
            file.Version.ShouldEqual(SemanticVersion.Parse("1.1.0.441"));
            file.IsPreRelease.ShouldBeFalse();
        }

        [Test]
        public void build_a_nuget_file_by_file_name_complex()
        {
			var file = fileFor("Bottles.Tools.1.1.0.441.nupkg");
            file.Name.ShouldEqual("Bottles.Tools");
            file.Version.ShouldEqual(SemanticVersion.Parse("1.1.0.441"));
            file.IsPreRelease.ShouldBeFalse();
        }

        [Test]
        public void build_a_nuget_file_by_file_name_for_alpha()
        {
			var file = fileFor("Bottles.1.0.0.441-alpha.nupkg");
            file.Name.ShouldEqual("Bottles");
            file.Version.ShouldEqual(SemanticVersion.Parse("1.0.0.441-alpha"));
            file.IsPreRelease.ShouldBeTrue();
        }

        [Test, Explicit]
        public void explode_smoke_test()
        {
            var file = new NugetFile("Bottles.1.0.0.441.nupkg");
            var system = new FileSystem();
            system.CreateDirectory("bottles");
            system.CleanDirectory("bottles");

            var package = file.ExplodeTo("bottles");

            package.ShouldNotBeNull();
            package.AssemblyReferences.Single().Name.ShouldEqual("Bottles.dll");
        }
    }
}