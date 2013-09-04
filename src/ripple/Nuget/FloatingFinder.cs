using System.Collections.Generic;
using System.Linq;
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
            IRemoteNuget nuget = null;
            var feeds = FeedRegistry.FloatedFeedsFor(solution).ToArray();
            foreach (var feed in feeds)
            {
                nuget = feed.FindLatest(dependency);
                if (nuget == null) continue;

                if (dependency.Mode == UpdateMode.Fixed && nuget.IsUpdateFor(dependency))
                {
                    nuget = null;
                    continue;
                }

                break;
            }

            if (nuget == null)
            {
                feeds.OfType<FloatingFileSystemNugetFeed>()
                    .Each(files => files.DumpLatest());
            }

            return NugetResult.For(nuget);
        }

        public void Filter(Solution solution, Dependency dependency, NugetResult result)
        {
            // no-op
        }
    }
}