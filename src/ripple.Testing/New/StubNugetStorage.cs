using System.Collections.Generic;
using ripple.New.Model;
using ripple.New.Nuget;
using ripple.Testing.New.Model;

namespace ripple.Testing.New
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

		public void Write(Solution solution)
		{
			throw new System.NotImplementedException();
		}

		public void Write(Project project)
		{
			throw new System.NotImplementedException();
		}

		public LocalDependencies Dependencies(Solution solution)
		{
			return new LocalDependencies(_files);
		}

		public IEnumerable<Dependency> MissingFiles(Solution solution)
		{
			yield break;
		}
	}
}