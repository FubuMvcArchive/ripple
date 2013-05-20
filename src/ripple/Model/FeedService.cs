using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FubuCore;
using FubuCore.Util;
using ripple.Nuget;

namespace ripple.Model
{
    public class FeedService : IFeedService
    {
        private readonly Solution _solution;
        private readonly IFeedConnectivity _connectivity;

        private readonly Cache<Dependency, IRemoteNuget> _nugetForCache = new Cache<Dependency, IRemoteNuget>();
        private readonly Dictionary<Dependency, IEnumerable<Dependency>> _dependenciesForFixedCache = new Dictionary<Dependency, IEnumerable<Dependency>>();
        private readonly Dictionary<Dependency, IEnumerable<Dependency>> _dependenciesForFloatCache = new Dictionary<Dependency, IEnumerable<Dependency>>();

        public FeedService(Solution solution, IFeedConnectivity connectivity)
        {
            _solution = solution;
            _connectivity = connectivity;

            _nugetForCache = new Cache<Dependency, IRemoteNuget>(x => nugetFor(x));

            ServicePointManager.DefaultConnectionLimit = 10;
        }

        public virtual IRemoteNuget NugetFor(Dependency dependency)
        {
            return _nugetForCache[dependency];
        }

        private IRemoteNuget nugetFor(Dependency dependency, bool retrying = false)
        {
            IRemoteNuget nuget = null;
            var feeds = _connectivity.FeedsFor(_solution);
            foreach (var feed in feeds)
            {
                _connectivity.IfOnline(feed, x => nuget = getLatestFromFloatingFeed(x, dependency));
                if (nuget != null) break;

                if (dependency.IsFloat() || dependency.Version.IsEmpty())
                {
                    _connectivity.IfOnline(feed, x => nuget = x.FindLatest(dependency));
                    if (nuget != null) break;
                }

                _connectivity.IfOnline(feed, x => nuget = x.Find(dependency));
                if (nuget != null) break;
            }

            if (nuget == null)
            {
                if (_connectivity.AllOffline(feeds) && !retrying)
                {
                    return nugetFor(dependency, true);
                }

                feeds.OfType<FloatingFileSystemNugetFeed>()
                    .Each(files => files.DumpLatest());

                RippleAssert.Fail("Could not find " + dependency);
            }

            return remoteOrCached(nuget);
        }

        private IRemoteNuget remoteOrCached(IRemoteNuget nuget)
        {
            if (nuget == null) return null;
            return _solution.Cache.Retrieve(nuget);
        }

        private IRemoteNuget getLatestFromFloatingFeed(INugetFeed feed, Dependency dependency)
        {
            var floatingFeed = feed as IFloatingFeed;
            if (floatingFeed == null) return null;

            var floatedResult = floatingFeed.GetLatest().SingleOrDefault(x => dependency.MatchesName(x.Name));
            if (floatedResult != null && dependency.Mode == UpdateMode.Fixed && floatedResult.IsUpdateFor(dependency))
            {
                return null;
            }

            return floatedResult;
        }

        // Almost entirely covered by integration tests
        public IEnumerable<Dependency> DependenciesFor(Dependency dependency, UpdateMode mode)
        {
            var cache = mode == UpdateMode.Fixed ? _dependenciesForFixedCache : _dependenciesForFloatCache;

            IEnumerable<Dependency> dependencies;
            if (cache.TryGetValue(dependency, out dependencies) == false)
            {
                dependencies = findDependenciesFor(dependency, mode);
                cache[dependency] = dependencies;
            }

            return dependencies.ToArray();
        }

        private IEnumerable<Dependency> findDependenciesFor(Dependency dependency, UpdateMode mode, int depth = 0)
        {
            var nuget = NugetFor(dependency);
            var dependencies = new List<Dependency>();

            if (depth != 0)
            {
                var dep = dependency;
                if (dep.IsFloat() && mode == UpdateMode.Fixed)
                {
                    dep = new Dependency(nuget.Name, nuget.Version, mode);
                }

                dependencies.Add(dep);
            }

            nuget
                .Dependencies()
                .Each(x => dependencies.AddRange(findDependenciesFor(x, mode, depth + 1)));

            return dependencies.OrderBy(x => x.Name);
        }

        public IRemoteNuget LatestFor(Solution solution, Dependency dependency, bool forced = false)
        {
            if (dependency.Mode == UpdateMode.Fixed && !forced)
            {
                return null;
            }

            IRemoteNuget latest = null;
            var feeds = _connectivity.FeedsFor(solution);

            foreach (var feed in feeds)
            {
                try
                {
                    IRemoteNuget nuget = null;
                    _connectivity.IfOnline(feed, x => nuget = feed.FindLatest(dependency));

                    if (latest == null)
                    {
                        latest = nuget;
                    }

                    if (latest != null && nuget != null && latest.Version < nuget.Version)
                    {
                        latest = nuget;
                    }
                }
                catch (Exception)
                {
                    RippleLog.Debug("Error while finding latest " + dependency);
                }
            }

            if (isUpdate(latest, dependency))
            {
                return remoteOrCached(latest);
            }

            return null;
        }

        private bool isUpdate(IRemoteNuget latest, Dependency dependency)
        {
            if (latest == null) return false;

            var localDependencies = _solution.LocalDependencies();
            if (localDependencies.Has(dependency))
            {
                var local = localDependencies.Get(dependency);
                return latest.IsUpdateFor(local);
            }

            if (dependency.IsFloat())
            {
                return true;
            }

            return latest.IsUpdateFor(dependency);
        }

        public static FeedService Basic(Solution solution)
        {
            return new FeedService(solution, new FeedConnectivity());
        }
    }
}