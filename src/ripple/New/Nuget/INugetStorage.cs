using System.Collections.Generic;
using System.IO;
using FubuCore;
using ripple.New.Model;

namespace ripple.New.Nuget
{
	public interface INugetStorage
	{
		// Gonna use this for the ripple clean command
		void Clean(Repository repository);

		// Don't need to expose the pathing information here
		void Download(Repository repository, INugetFile file);

		IEnumerable<NugetQuery> MissingFiles(Repository repository);
	}

	public class RippleStorage : INugetStorage
	{
		private readonly IFileSystem _fileSystem = new FileSystem();

		public void Clean(Repository repository)
		{
			_fileSystem.CleanDirectory(repository.PackagesDirectory());
		}

		public void Download(Repository repository, INugetFile file)
		{
			file.ExplodeTo(repository.PackagesDirectory());
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