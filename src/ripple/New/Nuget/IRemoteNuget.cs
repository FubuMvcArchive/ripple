using NuGet;

namespace ripple.New.Nuget
{
    public interface IRemoteNuget
    {
        string Name { get; }
        SemanticVersion Version { get; }
        INugetFile DownloadTo(string directory);
        string Filename { get; }
    }
}