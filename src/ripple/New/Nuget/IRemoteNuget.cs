using NuGet;
using ripple.New.Model;

namespace ripple.New.Nuget
{
    public interface IRemoteNuget
    {
        string Name { get; }
        SemanticVersion Version { get; }
        INugetFile DownloadTo(string directory);
        string Filename { get; }
	}

	public static class RemoteNugetExtensions
	{
		public static bool IsUpdateFor(this IRemoteNuget nuget, Dependency dependency)
		{
			return nuget.Version > dependency.SemanticVersion();
		}
	}
}