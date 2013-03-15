using NuGet;
using ripple.New.Model;

namespace ripple.New.Nuget
{
    public interface INugetFeed
    {
        IRemoteNuget Find(Dependency query);
		IRemoteNuget FindLatest(Dependency query);

		IPackageRepository Repository { get; }
    }
}