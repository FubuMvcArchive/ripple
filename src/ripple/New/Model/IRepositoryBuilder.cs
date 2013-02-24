using ripple.New.Nuget;

namespace ripple.New.Model
{
	public interface IRepositoryBuilder
	{
		Repository Build();
	}

	public class RepositoryBuilder : IRepositoryBuilder
	{
		private readonly IRepositoryFiles _files;
		private readonly IProjectReader _project;

		public RepositoryBuilder(IRepositoryFiles files, IProjectReader project)
		{
			_files = files;
			_project = project;
		}

		public Repository Build()
		{
			var repository = _files.LoadSolution();
			
			_files.ForProjects(x =>
			{
				var project = _project.Read(x);
				repository.AddProject(project);
			});

			// Maybe switch on the configure mode? Nuget vs. Ripple
			repository.UseStorage(new RippleStorage());

			return repository;
		}

		public static IRepositoryBuilder Basic()
		{
			return new RepositoryBuilder(new RepositoryFiles(), ProjectReader.Basic());
		}
	}
}