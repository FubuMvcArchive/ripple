using System.Diagnostics;
using FubuCore;
using NUnit.Framework;
using NuGet;
using System.Linq;
using FubuTestingSupport;
using System.Collections.Generic;
using ripple.New.Model;
using ripple.New.Nuget;

namespace ripple.Testing.New.Nuget
{
	[TestFixture, Explicit]
    public class NugetFolderCacheSmokeTester
    {
        private NugetFolderCache theCache;
        private FileSystem fileSystem;

        [SetUp]
        public void SetUp()
        {
            theCache = new NugetFolderCache(new Solution(), @"c:\nugets");
            fileSystem = new FileSystem();
        }

        [Test]
        public void flush()
        {
            fileSystem.WriteStringToFile(@"c:\nugets\foo.1.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\bar.1.0.0.0.nupkg", "foo");
        
            theCache.Flush();

            fileSystem.FindFiles("c:\\nugets", FileSet.Deep("*.*")).Any().ShouldBeFalse();
        }

        [Test]
        public void all_files()
        {
            theCache.Flush();

            fileSystem.WriteStringToFile(@"c:\nugets\foo.1.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\foo.1.1.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\foo.1.2.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\bar.1.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\bar.2.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\bar.3.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\lib1.1.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\lib2.1.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\lib3.1.0.0.0.nupkg", "foo");

            var files = theCache.AllFiles();
            files.Count().ShouldEqual(9);

            files.First(x => x.Name == "foo" && x.Version == SemanticVersion.Parse("1.0.0.0")).ShouldNotBeNull();
            files.First(x => x.Name == "bar" && x.Version == SemanticVersion.Parse("2.0.0.0")).ShouldNotBeNull();

        }

        [Test]
        public void find_latest()
        {
            theCache.Flush();

            fileSystem.WriteStringToFile(@"c:\nugets\foo.1.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\foo.1.1.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\foo.1.2.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\foo.1.3.0.0-alpha.nupkg", "foo");

            fileSystem.WriteStringToFile(@"c:\nugets\bar.1.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\bar.2.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\bar.3.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\lib1.1.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\lib2.1.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\lib3.1.0.0.0.nupkg", "foo");

            theCache.Latest(new Dependency("foo") { Stability = NugetStability.Anything})
                    .Version.ShouldEqual(SemanticVersion.Parse("1.3.0.0-alpha"));

            theCache.Latest(new Dependency("foo") { Stability = NugetStability.ReleasedOnly })
                    .Version.ShouldEqual(SemanticVersion.Parse("1.2.0.0"));

        }

        [Test]
        public void find()
        {
            theCache.Flush();

            fileSystem.WriteStringToFile(@"c:\nugets\foo.1.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\foo.1.1.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\foo.1.2.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\foo.1.3.0.0-alpha.nupkg", "foo");

            fileSystem.WriteStringToFile(@"c:\nugets\bar.1.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\bar.2.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\bar.3.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\lib1.1.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\lib2.1.0.0.0.nupkg", "foo");
            fileSystem.WriteStringToFile(@"c:\nugets\lib3.1.0.0.0.nupkg", "foo");

            theCache.Find(new Dependency("foo", "1.0.0.0")).ShouldNotBeNull();
			theCache.Find(new Dependency("foo", "1.0.0.0")).Version.Version.ToString()
                    .ShouldEqual("1.0.0.0");
			theCache.Find(new Dependency("foo", "1.1.0.0")).ShouldNotBeNull();
			theCache.Find(new Dependency("foo", "1.2.0.0")).ShouldNotBeNull();
			theCache.Find(new Dependency("foo", "1.3.0.0")).ShouldNotBeNull();
			theCache.Find(new Dependency("foo", "1.4.0.0")).ShouldBeNull();
        }


        [Test]
        public void update_all()
        {
            //theCache.Flush();
            
            var feed = new FloatingFeed(RippleConstants.FubuTeamCityFeed);
            theCache.UpdateAll(feed.GetLatest());

            fileSystem.FindFiles("c:\\nugets", FileSet.Deep("*.nupkg"))
                .Any().ShouldBeTrue();
        }

        [Test]
        public void get_all()
        {
            theCache.AllFiles().Each(file => Debug.WriteLine(file));
        }
    }
}