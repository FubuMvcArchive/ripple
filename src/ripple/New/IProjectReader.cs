using System.Collections.Generic;
using System.Linq;

namespace ripple.New
{
	public interface IProjectReader
	{
		Project Read(ProjectFiles files);
	}

	public class ProjectReader : IProjectReader
	{
		private readonly IEnumerable<IDependencyReader> _dependencies;

		public ProjectReader(IEnumerable<IDependencyReader> dependencies)
		{
			_dependencies = dependencies;
		}

		public Project Read(ProjectFiles files)
		{
			var project = new Project(files.ProjectFile);

			var reader = _dependencies.First(x => x.Matches(project, files.ProjectDir));
			var dependencies = reader.Read(project, files.ProjectDir);

			dependencies.Each(d => project.AddDependency(d));

			return project;
		}

		public static IProjectReader Basic()
		{
			return new ProjectReader(new IDependencyReader[] { new NuGetDependencyReader(), new RippleDependencyReader() });
		}
	}
}