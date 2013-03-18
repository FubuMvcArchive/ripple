using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Logging;
using NuGet;

namespace ripple.New.Model
{
	public class InstallationPlan : DescribesItself, LogTopic
	{
		private readonly Solution _solution;
		private readonly Project _project;
		private readonly Dependency _dependency;

		private readonly Lazy<IEnumerable<PackageDependency>> _dependencies;
		private readonly Lazy<IEnumerable<Dependency>> _installs;
		private readonly Lazy<IEnumerable<Dependency>> _updates;

		private InstallationPlan(Solution solution, Project project, Dependency dependency)
		{
			_solution = solution;
			_project = project;
			_dependency = dependency;

			_dependencies = new Lazy<IEnumerable<PackageDependency>>(findPackageDependencies);
			_installs = new Lazy<IEnumerable<Dependency>>(findDependenciesToInstall);
			_updates = new Lazy<IEnumerable<Dependency>>(findDependenciesToUpdate);
		}

		public void Execute()
		{
			Installations.Each(x => PackageInstallation.ForProject(_project.Name, x).InstallTo(_solution));
		}

		public virtual IEnumerable<Dependency> Installations
		{
			get { return _installs.Value; }
		}

		public virtual IEnumerable<Dependency> Updates
		{
			get { return _updates.Value; }
		}

		private IEnumerable<PackageDependency> dependencies { get { return _dependencies.Value; } }

		private IEnumerable<PackageDependency> findPackageDependencies()
		{
			return _solution.FeedService.DependenciesFor(_solution, _dependency);
		}

		private IEnumerable<Dependency> findDependenciesToInstall()
		{
			var allDependencies = new List<Dependency> { _dependency };

			dependencies
				.Where(x => !_project.Dependencies.Has(x.Id))
				.Each(x =>
				{
					var version = x.VersionSpec.MaxVersion ?? x.VersionSpec.MinVersion;
					allDependencies.Add(new Dependency(x.Id, version.ToString(), _dependency.Mode));
				});

			return allDependencies;
		}

		private IEnumerable<Dependency> findDependenciesToUpdate()
		{
			var updates = new List<Dependency>();
			var local = _solution.LocalDependencies();

			dependencies
				.Where(x => _solution.Dependencies.Has(x.Id))
				.Each(x =>
				{
					var configured = _solution.Dependencies.Find(x.Id);
					if (!configured.IsFloat())
					{
						// TODO -- warning?
						return;
					}

					if (local.Has(x.Id))
					{
						var localNuget = local.Get(configured);
						if (shouldUpdate(localNuget.Version, x))
						{
							updates.Add(configured);
						}
						return;
					}

					if (configured.Version.IsNotEmpty() && shouldUpdate(configured.SemanticVersion(), x))
					{
						updates.Add(configured);
					}
				});

			return updates;
		}

		private bool shouldUpdate(SemanticVersion version, PackageDependency dependency)
		{
			return !dependency.VersionSpec.Satisfies(version);
		}

		public static InstallationPlan Create(Solution solution, Project project, Dependency dependency)
		{
			var plan = new InstallationPlan(solution, project, dependency);
			return plan;
		}

		public void Describe(Description description)
		{
			if (Installations.Any())
			{
				var updates = description.AddList("Installations", Installations);
				updates.Label = "Installations";
			}

			if (Updates.Any())
			{
				var updates = description.AddList("Updates", Updates);
				updates.Label = "Updates";
			}
		}
	}	
}