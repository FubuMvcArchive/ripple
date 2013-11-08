using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuCore;
using FubuCore.Util;
using NuGet;
using ripple.Nuget;

namespace ripple.Model
{
    public interface IFeedProvider
    {
        INugetFeed For(Feed feed);

        IEnumerable<IFloatingFeed> FloatedFeedsFor(Solution solution);
        IEnumerable<INugetFeed> FeedsFor(Solution solution);
    }

    public class FeedProvider : IFeedProvider
    {
        private readonly Cache<Feed, INugetFeed> _feeds;

        public FeedProvider()
        {
            _feeds = new Cache<Feed, INugetFeed>(buildFeed);
        }

        public INugetFeed For(Feed feed)
        {
            return _feeds[feed];
        }

        public IEnumerable<IFloatingFeed> FloatedFeedsFor(Solution solution)
        {
            return FeedsFor(solution).OfType<IFloatingFeed>();
        }

        public IEnumerable<INugetFeed> FeedsFor(Solution solution)
        {
            return solution.Feeds.Select(For);
        }

        private INugetFeed buildFeed(Feed feed)
        {
            if (feed.Url.StartsWith("file://"))
            {
                return buildFileSystemFeed(feed);
            }

            if (!String.IsNullOrEmpty(feed.Username) && !String.IsNullOrEmpty(feed.Password))
            {
                if (HttpClient.DefaultCredentialProvider.GetType() != NugetCredentialsProvider.Instance.GetType())
                {
                    HttpClient.DefaultCredentialProvider = NugetCredentialsProvider.Instance;
                }
                NugetCredentialsProvider.Instance.AddCredentials(feed.Url, new NetworkCredential(feed.Username, feed.Password));
            }

            if (feed.Mode == UpdateMode.Fixed)
            {
                return new NugetFeed(feed.Url, feed.Stability);
            }

            return new FloatingFeed(feed.Url, feed.Stability);
        }

        private INugetFeed buildFileSystemFeed(Feed feed)
        {
            var directory = FeedRegistry.BranchUrl(feed);
            directory = directory.Replace("file://", "");
            directory = directory.ToFullPath();

            if (feed.Mode == UpdateMode.Fixed)
            {
                return new FileSystemNugetFeed(directory, feed.Stability);
            }

            return new FloatingFileSystemNugetFeed(directory, feed.Stability);
        }
    }

    public class FeedRegistry
    {
        private static IFeedProvider _provider;

        static FeedRegistry()
        {
            Reset();
        }

        public static void Stub(IFeedProvider provider)
        {
            _provider = provider;
        }

        public static void Reset()
        {
            Stub(new FeedProvider());
        }

        public static bool IsFloat(Solution solution, Dependency dependency)
        {
            return FloatedFeedsFor(solution).Any(feed => feed.FindLatest(dependency) != null);
        }

        public static INugetFeed FeedFor(Feed feed)
        {
            return _provider.For(feed);
        }

        public static INugetFeed CacheFor(Solution solution)
        {
            return solution.Cache.ToFeed().GetNugetFeed();
        }

        public static IEnumerable<IFloatingFeed> FloatedFeedsFor(Solution solution)
        {
            foreach (var feed in _provider.FloatedFeedsFor(solution))
            {
                if (feed.IsOnline())
                {
                    yield return feed;
                }
            }
        }

        public static IEnumerable<INugetFeed> FeedsFor(Solution solution)
        {
            foreach (var feed in _provider.FeedsFor(solution))
            {
                if (feed.IsOnline())
                {
                    yield return feed;
                }
            }
        }

        public const string BranchPlaceholder = "{branch}";

        public static string BranchUrl(Feed feed)
        {
            var branchUrl = feed.Url;
            if (branchUrl.Contains(BranchPlaceholder))
            {
                var branchName = BranchDetector.Current();
                branchUrl = branchUrl.Replace(BranchPlaceholder, branchName);

                RippleLog.Debug("Detected branch feed: {0}. Current branch is {1}. Setting url to {2}".ToFormat(feed, branchName, branchUrl), false);
            }

            return branchUrl;
        }
    }
}