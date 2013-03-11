using NuGet;

namespace ripple.New.Nuget
{
    public interface INugetFile
    {
		string FileName { get; }
        string Name { get; }
        SemanticVersion Version { get; }
        IPackage ExplodeTo(string directory);

	    INugetFile CopyTo(string directory);
    }
}