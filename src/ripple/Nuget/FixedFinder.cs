using ripple.Model;

namespace ripple.Nuget
{
    public class FixedFinder : INugetFinder
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
                nuget = feed.Find(dependency);
                if (nuget != null) break;
            }

            return NugetResult.For(nuget);
        }
    }
}