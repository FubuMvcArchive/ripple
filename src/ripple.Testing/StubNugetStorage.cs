using System.Collections.Generic;
using ripple.Model;
using ripple.Nuget;
using ripple.Testing.Model;

namespace ripple.Testing
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

		public void Clean(Solution solution, CleanMode mode)
		{
		}

		public void Write(Solution solution)
		{
		}

		public void Write(Project project)
		{
		}

		public void Reset(Solution solution)
		{
		}

		public LocalDependencies Dependencies(Solution solution)
		{
			return new LocalDependencies(_files);
		}

		public IEnumerable<Dependency> MissingFiles(Solution solution, bool force)
		{
			yield break;
		}
	}
}