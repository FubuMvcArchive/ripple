using System;
using FubuCore;
using NuGet;
using ripple.Model;

namespace ripple.Testing.Model
{
	public interface IInstallationScenario
	{
		InstallationPlan Build();
	}

	public class InstallationScenario : IInstallationScenario
	{
		private readonly Solution _solution = new Solution();
		private readonly Project _project = new Project("Test");
		private readonly Dependency _dependency;
		private readonly StubFeedService _service = new StubFeedService();
		private readonly StubNugetStorage _storage = new StubNugetStorage();

		public InstallationScenario(string name)
		{
			_dependency = new Dependency(name);

			_solution.AddProject(_project);
			_solution.UseFeedService(_service);
			_solution.UseStorage(_storage);
		}

		public void AddRemoteDependency(string id, string version)
		{
			AddRemoteDependency(id, version, x => { });
		}

		public void AddRemoteDependency(string id, string version, Action<PackageDependency> configure)
		{
			var dependency = new PackageDependency(id, new VersionSpec(SemanticVersion.Parse(version)));
			_service.AddPackageDependency(_dependency, dependency);
		}

		public void Solution(Action<Solution> configure)
		{
			configure(_solution);
		}

		public void Project(Action<Project> configure)
		{
			configure(_project);
		}

		public void AddLocalDependency(string name, string version, UpdateMode mode)
		{
			_project.AddDependency(new Dependency(name, version, mode));
			_storage.Add(name, version);
		}

		InstallationPlan IInstallationScenario.Build()
		{
			return InstallationPlan.Create(_solution, _project, _dependency);
		}

		public static InstallationPlan For(string name, Action<InstallationScenario> define)
		{
			var scenario = new InstallationScenario(name);
			define(scenario);

			return scenario.As<IInstallationScenario>().Build();
		}
	}
}