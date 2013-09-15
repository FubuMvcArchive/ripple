using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Util;
using ripple.Nuget;

namespace ripple.Model
{
    public class FeedService : IFeedService
    {
        private readonly Solution _solution;
        private readonly Cache<DependencyCacheKey, IEnumerable<Dependency>> _dependenciesCache = new Cache<DependencyCacheKey, IEnumerable<Dependency>>();

        public FeedService(Solution solution)
        {
            _solution = solution;
            _dependenciesCache.OnMissing = key => findDependenciesFor(key.Dependency, key.Mode, SearchLocation.Local);

            ServicePointManager.DefaultConnectionLimit = 10;
        }

        public virtual Task<NugetResult> NugetFor(Dependency dependency)
        {
            return NugetSearch.Find(_solution, dependency);
        }

        // Almost entirely covered by integration tests
        public IEnumerable<Dependency> DependenciesFor(Dependency dependency, UpdateMode mode, SearchLocation location = SearchLocation.Remote)
        {
            return _dependenciesCache[DependencyCacheKey.For(dependency, mode, location)];
        }

        private IEnumerable<Dependency> findDependenciesFor(Dependency dependency, UpdateMode mode, SearchLocation location)
        {
            var dependencies = new List<Dependency>();
            var task = findDependenciesFor(dependency, mode, 0, location, dependencies);

            try
            {
                task.Wait();
                if (!task.Result.Found)
                {
                    RippleAssert.Fail("Could not find " + dependency.Name);
                }
            }
            catch (AggregateException ex)
            {
                var flat = ex.Flatten();
                if (flat.InnerException != null)
                {
                    RippleLog.Debug(flat.InnerException.Message);
                }
            }

            return dependencies.OrderBy(x => x.Name);
        }

        private Task<NugetResult> findDependenciesFor(Dependency dependency, UpdateMode mode, int depth, SearchLocation location, List<Dependency> dependencies)
        {
            var result = findLocal(dependency, location);
            return Task.Factory.StartNew(() =>
            {
                if (result.Found)
                {
                    processNuget(result.Nuget, dependency, mode, depth, location, dependencies);
                    return result;
                }

                var search = NugetSearch.Find(_solution, dependency);

                search.ContinueWith(task =>
                {
                    if (!task.Result.Found)
                    {
                        return;
                    }

                    result.Import(task.Result);
                    var nuget = task.Result.Nuget;
                    processNuget(nuget, dependency, mode, depth, location, dependencies);

                }, TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.AttachedToParent);

                return result;

            });
        }

        private void processNuget(IRemoteNuget nuget, Dependency dependency, UpdateMode mode, int depth, SearchLocation location,
                                  List<Dependency> dependencies)
        {
            if (depth != 0)
            {
                var dep = dependency;
                var markAsFixed = mode == UpdateMode.Fixed || !FeedRegistry.IsFloat(_solution, dependency);

                if (dep.IsFloat() && markAsFixed)
                {
                    dep = new Dependency(nuget.Name, nuget.Version, UpdateMode.Fixed);
                }

                dependencies.Add(dep);
            }

            nuget
                .Dependencies()
                .Each(x => findDependenciesFor(x, mode, depth + 1, location, dependencies));
        }

        private NugetResult findLocal(Dependency dependency, SearchLocation location)
        {
            var result = new NugetResult();
            if (location == SearchLocation.Local && _solution.HasLocalCopy(dependency.Name))
            {
                try
                {
                    // Try to hit the local zip and read it. Mostly for testing but it'll detect a corrupted local package as well
                    result.Nuget = _solution.LocalNuget(dependency.Name);
                    result.Nuget.Dependencies().ToList();

                    RippleLog.Debug(dependency.Name + " already installed");
                }
                catch
                {
                    result.Nuget = null;
                }
            }

            return result;
        }

        public class DependencyCacheKey
        {
            public Dependency Dependency { get; private set; }
            public UpdateMode Mode { get; private set; }
            public SearchLocation Location { get; private set; }

            protected bool Equals(DependencyCacheKey other)
            {
                return Dependency.Equals(other.Dependency) && Mode == other.Mode;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((DependencyCacheKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Dependency.GetHashCode()*397) ^ (int) Mode;
                }
            }

            public static DependencyCacheKey For(Dependency dependency, UpdateMode mode, SearchLocation location)
            {
                return new DependencyCacheKey { Dependency = dependency, Mode = mode, Location = location};
            }
        }

        public static FeedService Basic(Solution solution)
        {
            return new FeedService(solution);
        }
    }
}