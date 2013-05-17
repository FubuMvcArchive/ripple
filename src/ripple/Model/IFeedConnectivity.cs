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
        private readonly IList<INugetFeed> _offline = new List<INugetFeed>();

        public void IfOnline(INugetFeed feed, Action<INugetFeed> continuation)
        {
            try
            {
                if (isOffline(feed))
                {
                    RippleLog.Debug("Feed offline. Ignoring.");
                    return;
                }

                continuation(feed);
            }
            catch (Exception exc)
            {
                markOffline(feed);
                RippleLog.Debug("Feed unavailable: " + feed);
                RippleLog.Debug(exc.ToString());
            }
        }

        public IEnumerable<INugetFeed> FeedsFor(Solution solution)
        {
            var feeds = solution.Feeds.Select(x => x.GetNugetFeed());
            if (!RippleConnection.Connected() || AllOffline(feeds))
            {
                var cache = solution.Cache.ToFeed();
                return new[] { cache.GetNugetFeed() };
            }

            return feeds;
        }

        public bool AllOffline(IEnumerable<INugetFeed> feeds)
        {
            return feeds.All(isOffline);
        }

        private void markOffline(INugetFeed feed)
        {
            _offline.Fill(feed);
        }

        private bool isOffline(INugetFeed feed)
        {
            return _offline.Contains(feed);
        }

        
    }
}