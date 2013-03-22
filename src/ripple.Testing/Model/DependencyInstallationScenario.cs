using System;
using FubuCore.Util;
using ripple.Model;

namespace ripple.Testing.Model
{
	public class DependencyInstallationScenario
	{
		public class ScenarioDefinition
		{
			public readonly Solution Solution = new Solution();
			private readonly Cache<string, Project> theProjects;

			public ScenarioDefinition()
			{
				theProjects = new Cache<string, Project>(name =>
				{
					var project = new Project(name);
					Solution.AddProject(project);

					return project;
				});
			}

			public void SolutionDependency(string name)
			{
				SolutionDependency(name, string.Empty);
			}

			public void SolutionDependency(string name, string version)
			{
				Solution.AddDependency(new Dependency(name, version));
			}

			public void ProjectDependency(string project, string name)
			{
				ProjectDependency(project, name, string.Empty);
			}

			public void ProjectDependency(string project, string name, string version)
			{
				theProjects[project].AddDependency(new Dependency(name, version));
			}
		}

		public static Solution Create(Action<ScenarioDefinition> configure)
		{
			var def = new ScenarioDefinition();
			configure(def);

			return def.Solution;
		}
	}
}