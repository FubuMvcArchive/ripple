using FubuCore;
using ripple.Nuget;

namespace ripple.Model
{
	public interface ISolutionBuilder
	{
		Solution Build();
	}

	public class SolutionBuilder : ISolutionBuilder
	{
		private readonly ISolutionFiles _files;
		private readonly IProjectReader _project;
		private readonly IFileSystem _fileSystem;

		public SolutionBuilder(ISolutionFiles files, IProjectReader project)
		{
			_files = files;
			_project = project;

			_fileSystem = new FileSystem();
		}

		public Solution Build()
		{
			var solution = _files.LoadSolution();
			
			_files.ForProjects(solution, x =>
			{
				var project = _project.Read(x);
				// Account of no packages/ripple config
				if (project != null)
				{
					solution.AddProject(project);
				}
			});


			solution.UseStorage(NugetStorage.For(solution.Mode));

			_fileSystem.CreateDirectory(solution.PackagesDirectory());

			_files.FinalizeSolution(solution);

			solution.Dependencies.MarkRead();

			return solution;
		}

		public static ISolutionBuilder Basic()
		{
			return new SolutionBuilder(SolutionFiles.Basic(), ProjectReader.Basic());
		}

		public static ISolutionBuilder Classic()
		{
			return new SolutionBuilder(SolutionFiles.Classic(), ProjectReader.Basic());
		}

		public static Solution ReadFromCurrentDirectory()
		{
			return ReadFrom(RippleFileSystem.FindSolutionDirectory());
		}

		public static Solution ReadFrom(string directory)
		{
			var builder = new SolutionBuilder(SolutionFiles.FromDirectory(directory), ProjectReader.Basic());
			return builder.Build();
		}

		public static ISolutionBuilder For(SolutionMode mode)
		{
			return mode == SolutionMode.Ripple ? Basic() : Classic();
		}
	}
}