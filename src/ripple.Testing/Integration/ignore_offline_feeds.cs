using System;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class ignore_offline_feeds
    {
        private FeedService theFeedService;
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            FeedScenario.Create(scenario =>
            {
                scenario.For(Feed.Fubu)
                    .Add("FubuCore", "1.1.0.0")
                    .ThrowWhenSearchingFor("Bottles", "1.0.0.0", new NotImplementedException());

                scenario.For(Feed.NuGetV2)
                        .Add("Bottles", "1.0.0.0");
            });

            theFeedService = new FeedService();

            theSolution = Solution.Empty();
            theSolution.AddFeed(Feed.Fubu);
            theSolution.AddFeed(Feed.NuGetV2);
        }

        [TearDown]
        public void TearDown()
        {
            FeedRegistry.Reset();
        }

        [Test]
        public void continues_to_next_feed_when_an_exception_occurs()
        {
            theFeedService.NugetFor(theSolution, new Dependency("Bottles", "1.0.0.0")).ShouldNotBeNull();
        }

        [Test]
        public void ignores_feed_after_encountering_error()
        {
            theFeedService.NugetFor(theSolution, new Dependency("Bottles", "1.0.0.0"));

            var dependency = new Dependency("FubuCore", "1.1.0.0");
            var exception = Exception<RippleFatalError>.ShouldBeThrownBy(() => theFeedService.NugetFor(theSolution, dependency));
            exception.Message.ShouldEqual("Could not find " + dependency);
        }

        [Test]
        public void continues_to_next_feed_when_finding_latest_and_an_exception_occurs()
        {
            var storage = new StubNugetStorage();
            storage.Add("Bottles", "0.9.9.9");

            theSolution.UseStorage(storage);

            theFeedService.LatestFor(theSolution, new Dependency("Bottles", "1.0.0.0")).ShouldNotBeNull();
        }
    }

    [TestFixture]
    public class fallback_to_cache_of_all_feeds_are_offline
    {
        private FeedService theFeedService;
        private Solution theSolution;

        [SetUp]
        public void SetUp()
        {
            theSolution = Solution.Empty();
            theSolution.AddFeed(Feed.Fubu);
            theSolution.AddFeed(Feed.NuGetV2);

            FeedScenario.Create(scenario =>
            {
                scenario.For(Feed.Fubu)
                    .Add("FubuCore", "1.1.0.0")
                    .ThrowWhenSearchingFor("Bottles", "1.0.0.0", new NotImplementedException());

                scenario.For(Feed.NuGetV2)
                    .ThrowWhenSearchingFor("Bottles", "1.0.0.0", new NotImplementedException());

                scenario.For(theSolution.Cache.ToFeed())
                    .Add("Bottles", "1.0.0.0");
            });

            theFeedService = new FeedService();
        }

        [TearDown]
        public void TearDown()
        {
            FeedRegistry.Reset();
        }

        [Test]
        public void verify_nuget()
        {
            theFeedService.NugetFor(theSolution, new Dependency("Bottles", "1.0.0.0")).ShouldNotBeNull();
        }
    }
}