using System.Collections.Generic;
using NuGet;
using ripple.New.Model;
using ripple.New.Nuget;

namespace ripple.Testing.New.Model
{
	public class StubNugetStorage : INugetStorage
	{
		private readonly IList<INugetFile> _files = new List<INugetFile>();

		public StubNugetFile Add(string id, string version)
		{
			var file = new StubNugetFile(new Dependency(id, version));
			_files.Add(file);

			return file;
		}

		public void Clean(Solution solution)
		{
		}

		public LocalDependencies Dependencies(Solution solution)
		{
			return new LocalDependencies(_files);
		}

		public IEnumerable<Dependency> MissingFiles(Solution solution)
		{
			yield break;
		}

		public class StubNugetFile : INugetFile
		{
			public StubNugetFile(Dependency dependency)
			{
				Name = dependency.Name;
				Version = SemanticVersion.Parse(dependency.Version);
			}

			public string Name { get; private set; }
			public SemanticVersion Version { get; private set; }

			public IPackage ExplodeTo(string directory)
			{
				throw new System.NotImplementedException();
			}
		}
	}
}