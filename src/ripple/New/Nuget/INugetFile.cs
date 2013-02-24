using NuGet;

namespace ripple.New.Nuget
{
    public interface INugetFile
    {
        string Name { get; }
        SemanticVersion Version { get; }
        IPackage ExplodeTo(string directory);
    }
}