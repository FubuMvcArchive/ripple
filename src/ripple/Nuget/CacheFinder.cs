using FubuCore;
using FubuCore.Descriptions;
using ripple.Model;

namespace ripple.Nuget
{
    public class CacheFinder : INugetFinder, DescribesItself
    {
        public bool Matches(Dependency dependency)
        {
            return dependency.Version.IsNotEmpty();
        }

        public NugetResult Find(Solution solution, Dependency dependency)
        {
            var cache = FeedRegistry.CacheFor(solution);
            var nuget = cache.Find(dependency);

            return new NugetResult { Nuget = nuget };
        }

        public void Describe(Description description)
        {
            description.ShortDescription = "Search in Cache";
        }
    }
}