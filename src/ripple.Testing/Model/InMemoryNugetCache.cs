using System.Collections.Generic;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Model
{
    public class InMemoryNugetCache : INugetCache
    {
        private readonly Feed _feed;

        public InMemoryNugetCache(Feed feed)
        {
            _feed = feed;
        }

        public void Update(IRemoteNuget nuget)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateAll(IEnumerable<IRemoteNuget> nugets)
        {
            throw new System.NotImplementedException();
        }

        public INugetFile Latest(Dependency query)
        {
            throw new System.NotImplementedException();
        }

        public void Flush()
        {
            throw new System.NotImplementedException();
        }

        public INugetFile Find(Dependency query)
        {
            throw new System.NotImplementedException();
        }

        public IRemoteNuget Retrieve(IRemoteNuget nuget)
        {
            return nuget;
        }

        public Feed ToFeed()
        {
            return _feed;
        }

        public string LocalPath { get; private set; }
    }
}