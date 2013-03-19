using NuGet;
using ripple.New.Model;
using ripple.New.Nuget;

namespace ripple.Testing.New
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
			throw new System.NotImplementedException();
		}

		public string Filename { get; private set; }
	}
}