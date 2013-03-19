using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using ripple.New.Model;
using ripple.New.Nuget;

namespace ripple.Testing.New.Nuget
{
    [TestFixture, Explicit]
    public class NugetFeedSmokeTester
    {
        [Test]
        public void find_nuget_by_name()
        {
            var feed = new NugetFeed(RippleConstants.NugetOrgFeed.First());
			feed.Find(new Dependency("FubuMVC.Core","1.0.0.1402")).ShouldNotBeNull();
        }

        [Test]
        public void find_latest_by_name()
        {
            var feed = new NugetFeed(RippleConstants.NugetOrgFeed.First());
			feed.FindLatest(new Dependency("FubuMVC.Core"))
                .Version.Version.ToString().ShouldEqual("1.0.0.1402");
        }

        [Test]
        public void find_latest_by_name_then_download_it()
        {
            var feed = new NugetFeed(RippleConstants.NugetOrgFeed.First());
			var nuget = feed.FindLatest(new Dependency("FubuMVC.Core"));
            nuget.DownloadTo(null, "");
        }
    }
}