using System;
using System.Collections.Generic;
using System.Linq;
using ripple.Nuget;

namespace ripple.Model
{
    public interface IFeedConnectivity
    {
        void IfOnline(INugetFeed feed, Action<INugetFeed> continuation);
        bool AllOffline(IEnumerable<INugetFeed> feeds);
        IEnumerable<INugetFeed> FeedsFor(Solution solution);
    }

    // Covered with integration tests
    public class FeedConnectivity : IFeedConnectivity
    {
        private readonly IList<INugetFeed> _checked = new List<INugetFeed>(); 
        private readonly IList<INugetFeed> _offline = new List<INugetFeed>();

        public void IfOnline(INugetFeed feed, Action<INugetFeed> continuation)
        {
            try
            {
                if (isOffline(feed))
                {
                    RippleLog.Debug("Feed offline. Ignoring " + feed);
                    return;
                }

                continuation(feed);
            }
            catch (Exception exc)
            {
                MarkOffline(feed);
                RippleLog.Info("Feed unavailable: " + feed);
                RippleLog.Debug(exc.ToString());
            }
        }

        public IEnumerable<INugetFeed> FeedsFor(Solution solution)
        {
            var cache = solution.Cache.ToFeed().GetNugetFeed();
            var feeds = new List<INugetFeed> { cache };
            var solutionFeeds = solution.Feeds.Select(x => x.GetNugetFeed()).ToArray();

            if (RippleEnvironment.Connected() && !AllOffline(solutionFeeds))
            {
                feeds.AddRange(solutionFeeds);
            }

            
            return feeds;
        }

        public bool AllOffline(IEnumerable<INugetFeed> feeds)
        {
            return feeds.All(isOffline);
        }

        public void MarkOffline(INugetFeed feed)
        {
            _offline.Fill(feed);
        }

        private bool isOffline(INugetFeed feed)
        {
            if (!_checked.Contains(feed))
            {
                if (!feed.IsOnline())
                {
                    _offline.Add(feed);
                }

                _checked.Add(feed);
            }

            return _offline.Contains(feed);
        }
    }
}