using System.Collections.Generic;
using NuGet;
using ripple.Model;

namespace ripple.Nuget
{
    public abstract class NugetFeedBase : INugetFeed
    {
        private static readonly NullRemoteNuget Null = new NullRemoteNuget();
        private readonly Dictionary<Dependency, IRemoteNuget> _findCache = new Dictionary<Dependency, IRemoteNuget>();
        private readonly Dictionary<string, IRemoteNuget> _findLatest = new Dictionary<string, IRemoteNuget>();
        
        public IRemoteNuget Find(Dependency query)
        {
            IRemoteNuget nuget;
            if (_findCache.TryGetValue(query, out nuget) == false)
            {
                nuget = FindImpl(query);
                _findCache[query] = nuget ?? Null;
            }

            return Return(nuget);
        }

        protected abstract IRemoteNuget FindImpl(Dependency query);

        public IRemoteNuget FindLatest(Dependency query)
        {
            IRemoteNuget nuget;
            if (_findLatest.TryGetValue(query.Name, out nuget) == false)
            {
                nuget = FindLatestImpl(query);

                _findLatest[query.Name] = nuget ?? Null;

                if (nuget != null && nuget.Version != null)
                {
                    var key = new Dependency(nuget.Name, nuget.Version.ToString());

                    if (_findCache.ContainsKey(key) == false)
                        _findCache[key] = nuget;
                }
            }

            return Return(nuget);
        }

        protected abstract IRemoteNuget FindLatestImpl(Dependency query);

        private static IRemoteNuget Return(object nuget)
        {
            if (ReferenceEquals(Null, nuget))
                return null;

            return (IRemoteNuget) nuget;
        }

        public abstract IPackageRepository Repository { get; }

        private class NullRemoteNuget : IRemoteNuget
        {
            public string Name { get; private set; }
            public SemanticVersion Version { get; private set; }
            public INugetFile DownloadTo(Solution solution, string directory)
            {
                throw new System.NotImplementedException();
            }

            public string Filename { get; private set; }
            public IEnumerable<Dependency> Dependencies()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}