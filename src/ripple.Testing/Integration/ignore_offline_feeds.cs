using System;
using FubuCore;
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
                    .ThrowWhenSearchingFor("Bottles", "1.0.0.0", UpdateMode.Float, new NotImplementedException());

                scenario.For(Feed.NuGetV2)
                        .Add("Bottles", "1.0.0.0");
            });

            theSolution = Solution.Empty();
            theSolution.AddFeed(Feed.Fubu);
            theSolution.AddFeed(Feed.NuGetV2);

            theFeedService = theSolution.FeedService.As<FeedService>();
        }

        [TearDown]
        public void TearDown()
        {
            FeedRegistry.Reset();
        }

        [Test]
        public void continues_to_next_feed_when_an_exception_occurs()
        {
            var task = theFeedService.NugetFor(new Dependency("Bottles", "1.0.0.0"));
            task.Wait();

            task.Result.Nuget.ShouldNotBeNull();
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

            theFeedService = theSolution.FeedService.As<FeedService>();
        }

        [TearDown]
        public void TearDown()
        {
            FeedRegistry.Reset();
        }

        [Test]
        public void verify_nuget()
        {
            theFeedService.NugetFor(new Dependency("Bottles", "1.0.0.0")).ShouldNotBeNull();
        }
    }
}