using System.Collections.Generic;
using System.Linq;

namespace ripple.New.Model
{
	public interface IProjectReader
	{
		Project Read(string projectFile);
	}

	public class ProjectReader : IProjectReader
	{
		private readonly IEnumerable<IDependencyReader> _dependencies;

		public ProjectReader(IEnumerable<IDependencyReader> dependencies)
		{
			_dependencies = dependencies;
		}

		public Project Read(string projectFile)
		{
			var project = new Project(projectFile);

			var reader = _dependencies.First(x => x.Matches(project));
			var dependencies = reader.Read(project);

			dependencies.Each(d => project.Dependencies.Add(d));

			return project;
		}

		public static IProjectReader Basic()
		{
			return new ProjectReader(new IDependencyReader[] { new NuGetDependencyReader(), new RippleDependencyReader() });
		}
	}
}