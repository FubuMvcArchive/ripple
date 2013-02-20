namespace ripple.New
{
	public interface ISolutionBuilder
	{
		Solution Build();
	}

	public class SolutionBuilder : ISolutionBuilder
	{
		private readonly ISolutionFiles _files;
		private readonly IProjectReader _project;

		public SolutionBuilder(ISolutionFiles files, IProjectReader project)
		{
			_files = files;
			_project = project;
		}

		public Solution Build()
		{
			var solution = _files.LoadSolution();
			
			_files.ForProjects(x =>
			{
				var project = _project.Read(x);
				solution.AddProject(project);
			});

			return solution;
		}
	}
}