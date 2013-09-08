using FubuCore;
using ripple.Model;

namespace ripple.Nuget
{
    public class DefaultFinder : INugetFinder
    {
        public bool Matches(Dependency dependency)
        {
            return true;
        }

        public NugetResult Find(Solution solution, Dependency dependency)
        {
            IRemoteNuget nuget = null;
            var feeds = FeedRegistry.FeedsFor(solution);
            foreach (var feed in feeds)
            {
                nuget = dependency.Version.IsEmpty() ? feed.FindLatest(dependency) : feed.Find(dependency);

                if (nuget != null) break;
            }

            return NugetResult.For(nuget);
        }

        public NugetResult Latest(Solution solution, Dependency dependency)
        {
            IRemoteNuget nuget = null;
            var feeds = FeedRegistry.FeedsFor(solution);
            foreach (var feed in feeds)
            {
                nuget = feed.FindLatest(dependency);
                if (nuget != null) break;
            }

            return NugetResult.For(nuget);
        }

        public void Filter(Solution solution, Dependency dependency, NugetResult result)
        {
            if (!dependency.IsFloat() || !result.Found) return;

            var latest = Latest(solution, dependency);
            if (!latest.Found)
            {
                return;
            }

            if (latest.Nuget.Version > result.Nuget.Version)
            {
                result.Import(latest);
            }
        }
    }
}