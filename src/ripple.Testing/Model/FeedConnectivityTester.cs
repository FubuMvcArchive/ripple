using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class FeedConnectivityTester
    {
        private FeedConnectivity theConnectivity;
        private Solution theSolution;
        
        private Feed theFeed;
        private Feed theCacheFeed;

        [SetUp]
        public void SetUp()
        {
            theSolution = Solution.Empty();
            
            theCacheFeed = new Feed("cache");
            theFeed = new Feed("test");

            theSolution.AddFeed(theFeed);
            theSolution.UseCache(new InMemoryNugetCache(theCacheFeed));

            FeedScenario.Create(scenario =>
            {
                scenario.For(theFeed);
                scenario.For(theCacheFeed);
            });

            theConnectivity = new FeedConnectivity();
        }

        [TearDown]
        public void TearDown()
        {
            RippleEnvironment.Live();
            FeedRegistry.Reset();
        }

        [Test]
        public void offline_just_returns_the_cache()
        {
            RippleEnvironment.StubConnection(false);
            var cachedFeed = theCacheFeed.GetNugetFeed();
            theConnectivity.FeedsFor(theSolution).ShouldHaveTheSameElementsAs(cachedFeed);
        }

        [Test]
        public void all_offline_just_returns_the_cache()
        {
            RippleEnvironment.StubConnection(true);

            var testFeed = theFeed.GetNugetFeed();
            theConnectivity.MarkOffline(testFeed);

            var cachedFeed = theCacheFeed.GetNugetFeed();

            theConnectivity.FeedsFor(theSolution).ShouldHaveTheSameElementsAs(cachedFeed);
        }

        [Test]
        public void online_returns_the_cache_first_and_the_feed()
        {
            RippleEnvironment.StubConnection(true);

            var testFeed = theFeed.GetNugetFeed();
            var cachedFeed = theCacheFeed.GetNugetFeed();

            theConnectivity.FeedsFor(theSolution).ShouldHaveTheSameElementsAs(cachedFeed, testFeed);
        }
    }
}