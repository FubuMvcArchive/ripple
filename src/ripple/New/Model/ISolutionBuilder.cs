using FubuCore;
using ripple.New.Nuget;

namespace ripple.New.Model
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
			
			_files.ForProjects(x =>
			{
				var project = _project.Read(x);
				solution.AddProject(project);
			});

			// TODO -- Maybe switch on the configured mode? Nuget vs. Ripple
			solution.UseStorage(RippleStorage.Basic());

			_fileSystem.CreateDirectory(solution.PackagesDirectory());

			return solution;
		}

		public static ISolutionBuilder Basic()
		{
			return new SolutionBuilder(SolutionFiles.Basic(), ProjectReader.Basic());
		}
	}
}