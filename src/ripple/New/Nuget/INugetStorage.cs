using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using ripple.New.Model;

namespace ripple.New.Nuget
{
	public interface INugetStorage
	{
		// Gonna use this for the ripple clean command
		void Clean(Repository repository);

		LocalDependencies Dependencies(Repository repository);

		IEnumerable<NugetQuery> MissingFiles(Repository repository);
	}

	public class RippleStorage : INugetStorage
	{
		private readonly IFileSystem _fileSystem = new FileSystem();

		public void Clean(Repository repository)
		{
			_fileSystem.CleanDirectory(repository.PackagesDirectory());
		}

		public LocalDependencies Dependencies(Repository repository)
		{
			var nupkgSet = new FileSet
			{
				DeepSearch = true,
				Include = "*.nupkg"
			};

			return new LocalDependencies(_fileSystem
				.FindFiles(repository.PackagesDirectory(), nupkgSet)
				.Select(x => new NugetFile(x)).ToList());
		}

		public IEnumerable<NugetQuery> MissingFiles(Repository repository)
		{
			var missing = new List<NugetQuery>();

			repository.AllDependencies().Each(nuget =>
			{
				var packages = repository.PackagesDirectory();
				var packageDir = Path.Combine(packages, nuget.Name);

				if (!_fileSystem.DirectoryExists(packageDir))
				{
					missing.Fill(NugetQuery.For(nuget));
				}
			});

			return missing;
		}
	}
}