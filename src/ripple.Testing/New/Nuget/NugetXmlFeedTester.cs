using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;
using ripple.New.Nuget;

namespace ripple.Testing.New.Nuget
{
    [TestFixture]
    public class NugetXmlFeedTester
    {
        private NugetXmlFeed theFeed;

        [SetUp]
        public void SetUp()
        {
            theFeed = NugetXmlFeed.LoadFrom("feed.xml");
        }

        [Test]
        public void does_not_blow_up()
        {
            theFeed.ReadAll().Any().ShouldBeTrue();
        }

        [Test]
        public void spot_check_a_nuget()
        {
            var nuget = theFeed.ReadAll().Single(x => x.Name == "FubuMVC.Core").ShouldBeOfType<RemoteNuget>();

            nuget.Name.ShouldEqual("FubuMVC.Core");
            nuget.Version.Version.ToString().ShouldEqual("1.0.0.1402");
            nuget.Downloader.ShouldBeOfType<UrlNugetDownloader>().Url.ShouldEqual("http://build.fubu-project.org/guestAuth/repository/download/bt3/12671:id/FubuMVC.Core.1.0.0.1402.nupkg");
            nuget.Filename.ShouldEqual("FubuMVC.Core.1.0.0.1402.nupkg");
        }
    }
}