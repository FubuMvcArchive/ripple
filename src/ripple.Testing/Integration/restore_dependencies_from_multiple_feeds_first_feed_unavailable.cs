using System;
using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class restore_dependencies_from_multiple_feeds_first_feed_unavailable
    {
        private SolutionScenario theScenario;
        private Solution theSolution;
        private Feed theUnavailableFeed;
        private Feed anotherFloatingFeed;

        [SetUp]
        public void SetUp()
        {
            theUnavailableFeed = new Feed("unavailable");
            anotherFloatingFeed = new Feed("floated");

            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                {
                    test.SolutionDependency("Bottles", "0.9.0.1", UpdateMode.Float);
                    test.SolutionDependency("FubuJson", "0.9.0.6", UpdateMode.Float);
                });
            });

            theSolution = theScenario.Find("Test");
            theSolution.ClearFeeds();

            theSolution.AddFeed(theUnavailableFeed);
            theSolution.AddFeed(Feed.Fubu);
            theSolution.AddFeed(anotherFloatingFeed);


            FeedScenario.Create(scenario =>
            {
                scenario
                    .For(theSolution.Cache.ToFeed())
                    .Add("Bottles", "0.8.0.123")
                    .Add("FubuJson", "0.9.0.1");

                scenario
                    .For(theUnavailableFeed)
                    .ThrowWhenSearchingFor("FubuJson", new InvalidOperationException("DNS Error"));

                scenario
                    .For(Feed.Fubu)
                    .Add("Bottles", "0.9.0.1")
                    .Add("FubuJson", "0.9.0.333");

                scenario
                    .For(anotherFloatingFeed)
                    .Add("Test", "1.0.0.0");

                scenario
                    .For(Feed.NuGetV2)
                    .Add("FubuTransportation", "0.9.0.1");
            });

            RippleOperation
                .With(theSolution)
                .Execute<RestoreInput, RestoreCommand>(x => x.VerboseFlag = true);
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void restores_all_dependencies()
        {
            var local = theSolution.LocalDependencies();

            local.Get("Bottles").Version.ShouldEqual(new SemanticVersion("0.9.0.1"));
            local.Get("FubuJson").Version.ShouldEqual(new SemanticVersion("0.9.0.333"));
        }
    }
}