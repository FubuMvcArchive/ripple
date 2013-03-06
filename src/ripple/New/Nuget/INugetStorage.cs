using System.Collections.Generic;
using System.Linq;
using FubuCore;
using ripple.New.Model;

namespace ripple.New.Nuget
{
	public interface INugetStorage
	{
		// Gonna use this for the ripple clean command
		void Clean(Solution solution);

		LocalDependencies Dependencies(Solution solution);

		IEnumerable<Dependency> MissingFiles(Solution solution);
	}

	public class RippleStorage : INugetStorage
	{
		private readonly IFileSystem _fileSystem = new FileSystem();

		public void Clean(Solution solution)
		{
			_fileSystem.CleanDirectory(solution.PackagesDirectory());
		}

		public LocalDependencies Dependencies(Solution solution)
		{
			var nupkgSet = new FileSet
			{
				DeepSearch = true,
				Include = "*.nupkg"
			};

			var files = _fileSystem
				.FindFiles(solution.PackagesDirectory(), nupkgSet)
				.Select(x => new NugetFile(x)).ToList();

			return new LocalDependencies(files);
		}

		public IEnumerable<Dependency> MissingFiles(Solution solution)
		{
			var dependencies = Dependencies(solution);

			return solution
				.Dependencies
				.Where(dependency => !dependencies.Has(dependency));
		}
	}
}