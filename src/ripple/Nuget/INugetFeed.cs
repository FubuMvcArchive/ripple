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
        IRemoteNuget FindLatestByName(string name);
        IEnumerable<IRemoteNuget> FindAllLatestByName(string idPart);

        IPackageRepository Repository { get; }
    }
}