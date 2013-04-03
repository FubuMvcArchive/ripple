using FubuCore;
using NuGet;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing
{
	public class StubNuget : IRemoteNuget
	{
		public StubNuget(Dependency dependency)
			: this(dependency.Name, dependency.Version)
		{
		}

		public StubNuget(string name, string version)
		{
			Name = name;
			Version = SemanticVersion.Parse(version);
		}

		public string Name { get; private set; }
		public SemanticVersion Version { get; private set; }

		public INugetFile DownloadTo(Solution solution, string directory)
		{
			var files = new FileSystem();
			files.CreateDirectory(directory);

			var fileName = "{0}.{1}.nupkg".ToFormat(Name, Version);
			files.WriteStringToFile(fileName, "");

			return new NugetFile(fileName, SolutionMode.Ripple);
		}

		public string Filename { get; private set; }
	}
}