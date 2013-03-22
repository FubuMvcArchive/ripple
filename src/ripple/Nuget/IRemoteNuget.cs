using NuGet;
using ripple.Model;

namespace ripple.Nuget
{
    public interface IRemoteNuget
    {
        string Name { get; }
        SemanticVersion Version { get; }
        INugetFile DownloadTo(Solution solution, string directory);
        string Filename { get; }
	}

	public static class RemoteNugetExtensions
	{
		public static bool IsUpdateFor(this IRemoteNuget nuget, Dependency dependency)
		{
			return nuget.Version > dependency.SemanticVersion();
		}

		public static bool IsUpdateFor(this IRemoteNuget nuget, INugetFile dependency)
		{
			return nuget.Version > dependency.Version;
		}

		public static Dependency ToDependency(this IRemoteNuget nuget, UpdateMode mode = UpdateMode.Float)
		{
			return new Dependency(nuget.Name, nuget.Version.ToString(), mode);
		}
	}
}