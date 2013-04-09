namespace ripple.Model
{
	public enum InstallationTarget
	{
		Solution,
		Project
	}

	public class PackageInstallation
	{
		public InstallationTarget Target { get; private set; }
		public string Project { get; private set; }
		public Dependency Dependency { get; private set; }

		public void InstallTo(Solution solution)
		{
			if (Target == InstallationTarget.Solution)
			{
				solution.AddDependency(Dependency);
				return;
			}

			installToProject(solution);
		}

		private void installToProject(Solution solution)
		{
			RippleLog.Debug("Installing " + Dependency);

			var project = solution.FindProject(Project);
			project.AddDependency(new Dependency(Dependency.Name));

			if (!Dependency.IsFloat())
			{
				solution.AddDependency(Dependency);
			}
		}

		public static PackageInstallation ForSolution(Dependency dependency)
		{
			return new PackageInstallation { Dependency = dependency, Target = InstallationTarget.Solution };
		}

		public static PackageInstallation ForProject(string project, Dependency dependency)
		{
			return new PackageInstallation
			{
				Dependency = dependency,
				Project = project,
				Target = InstallationTarget.Project
			};
		}

        public static PackageInstallation ForProject(Project project, Dependency dependency)
        {
            return ForProject(project.Name, dependency);
        }
	}
}