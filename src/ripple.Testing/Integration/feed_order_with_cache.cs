using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;
using ripple.Testing.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class feed_order_with_cache
    {
        private SolutionScenario theScenario;
        private Solution theSolution;
        private Feed theCacheFeed;
        private Feed theFileSystemFeed;

        [SetUp]
        public void SetUp()
        {
            theCacheFeed = new Feed("file://cache");
            theFileSystemFeed = new Feed("file://feed");

            FeedScenario.Create(scenario =>
            {
                scenario.For(theCacheFeed)
                        .Add("Dependency1", "1.0.0.0");

                scenario.For(theFileSystemFeed)
                        .Add("Dependency1", "1.1.0.0");

                scenario.For(Feed.NuGetV2)
                        .Add("Dependency1", "1.0.23.0");
            });

            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                {
                    test.SolutionDependency("Dependency1", "1.1.0.0", UpdateMode.Float);
                });
            });

            theSolution = theScenario.Find("Test");
            theSolution.ClearFeeds();
            theSolution.AddFeed(theFileSystemFeed);
            theSolution.AddFeed(Feed.NuGetV2);

            theSolution.UseCache(new InMemoryNugetCache(theCacheFeed));
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void restores_the_latest_version()
        {
            var missing = theSolution.MissingNugets().Single();
            var target = theSolution.Restore(missing);
            target.Wait();

            target.Result.Nuget.Version.ShouldEqual(new SemanticVersion("1.1.0.0"));
        } 
    }

    [TestFixture]
    public class floating_feed_order_with_cache
    {
        private SolutionScenario theScenario;
        private Solution theSolution;
        private Feed theCacheFeed;
        private Feed theFileSystemFeed;

        [SetUp]
        public void SetUp()
        {
            theCacheFeed = new Feed("file://cache");
            theFileSystemFeed = new Feed("file://feed", UpdateMode.Float);

            FeedScenario.Create(scenario =>
            {
                scenario.For(theCacheFeed)
                        .Add("Dependency1", "1.0.0.0");

                scenario.For(theFileSystemFeed)
                        .Add("Dependency1", "1.1.0.0");

                scenario.For(Feed.NuGetV2)
                        .Add("Dependency1", "1.0.23.0");
            });

            theScenario = SolutionScenario.Create(scenario =>
            {
                scenario.Solution("Test", test =>
                {
                    test.SolutionDependency("Dependency1", "1.1.0.0", UpdateMode.Float);
                });
            });

            theSolution = theScenario.Find("Test");
            theSolution.ClearFeeds();
            theSolution.AddFeed(theFileSystemFeed);
            theSolution.AddFeed(Feed.NuGetV2);

            theSolution.UseCache(new InMemoryNugetCache(theCacheFeed));
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void restores_the_latest_version()
        {
            var missing = theSolution.MissingNugets().Single();
            var target = theSolution.Restore(missing);
            target.Wait();

            target.Result.Nuget.Version.ShouldEqual(new SemanticVersion("1.1.0.0"));
        }
    }
}