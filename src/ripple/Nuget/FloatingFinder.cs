using System.Collections.Generic;
using System.Linq;
using FubuCore;
using ripple.Model;

namespace ripple.Nuget
{
    public class FloatingFinder : INugetFinder
    {
        public bool Matches(Dependency dependency)
        {
            return dependency.IsFloat();
        }

        public NugetResult Find(Solution solution, Dependency dependency)
        {
            var feeds = FeedRegistry.FloatedFeedsFor(solution).ToArray();
            var result = NugetSearch.FindNuget(feeds, x =>
            {
                var feed = x.As<IFloatingFeed>();
                var nuget = feed.FindLatest(dependency);;

                if (nuget != null && dependency.Mode == UpdateMode.Fixed && nuget.IsUpdateFor(dependency))
                {
                    return null;
                }

                return nuget;
            });

            if (!result.Found)
            {
                feeds.OfType<FloatingFileSystemNugetFeed>()
                    .Each(files => files.DumpLatest());
            }

            return result;
        }
    }
}