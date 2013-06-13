using System.Collections.Generic;
using NuGet;
using ripple.Model;

namespace ripple.Nuget
{
    public interface INugetFeed
    {
        bool IsOnline();

        IRemoteNuget Find(Dependency query);
        IRemoteNuget FindLatest(Dependency query);

        IEnumerable<IRemoteNuget> FindLatestByNamePrefix(string idPrefix);

        IPackageRepository Repository { get; }
    }
}