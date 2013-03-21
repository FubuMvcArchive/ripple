using NuGet;
using ripple.New.Model;
using ripple.New.Nuget;

namespace ripple.Testing.New.Model
{
	public class StubNugetFile : INugetFile
	{
		public StubNugetFile(Dependency dependency)
		{
			Name = dependency.Name;
			Version = SemanticVersion.Parse(dependency.Version);
		}

		public string FileName { get; private set; }
		public string Name { get; private set; }
		public SemanticVersion Version { get; private set; }

		public IPackage ExplodeTo(string directory)
		{
			throw new System.NotImplementedException();
		}

		public INugetFile CopyTo(string directory)
		{
			throw new System.NotImplementedException();
		}

		public string NugetFolder(Solution solution)
		{
			throw new System.NotImplementedException();
		}
	}
}