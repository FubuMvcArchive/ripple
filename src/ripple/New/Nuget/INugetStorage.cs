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

		void Write(Solution solution);
		void Write(Project project);

		LocalDependencies Dependencies(Solution solution);

		IEnumerable<Dependency> MissingFiles(Solution solution);
	}

	public class RippleStorage : INugetStorage
	{
		private readonly IFileSystem _fileSystem;
		private readonly IDependencyStrategy _strategy;

		public RippleStorage(IFileSystem fileSystem, IDependencyStrategy strategy)
		{
			_fileSystem = fileSystem;
			_strategy = strategy;
		}

		public void Clean(Solution solution)
		{
			_fileSystem.CleanDirectory(solution.PackagesDirectory());
		}

		public void Write(Solution solution)
		{
			_fileSystem.PersistToFile(solution, solution.Path);
		}

		public void Write(Project project)
		{
			_strategy.Write(project);
			project.CsProj.Write();
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

		public static RippleStorage Basic()
		{
			return new RippleStorage(new FileSystem(), new RippleDependencyStrategy());
		}
	}
}