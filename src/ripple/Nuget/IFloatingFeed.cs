using System.Collections.Generic;
using ripple.Model;

namespace ripple.Nuget
{
    public interface IFloatingFeed : INugetFeed
    {
        IEnumerable<IRemoteNuget> GetLatest();
        IRemoteNuget LatestFor(Dependency dependency);
    }
}