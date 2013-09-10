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

        IEnumerable<IRemoteNuget> FindAllLatestByName(string idPart);

        IRemoteNuget FindLatestByName(string name);

        IPackageRepository Repository { get; }
    }
}