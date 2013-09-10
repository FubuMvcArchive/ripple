using System.Collections.Generic;
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

        IEnumerable<Dependency> Dependencies();
    }

    public static class RemoteNugetExtensions
    {
        public static bool IsUpdateFor(this IRemoteNuget nuget, Dependency dependency)
        {
            var version = dependency.SemanticVersion();
            if (version == null) return false;

            return nuget.Version > version;
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