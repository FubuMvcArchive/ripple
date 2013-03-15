using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using FubuCore;
using NuGet;
using ripple.New.Nuget;

namespace ripple.New.Model
{
	public interface IFeedService
	{
		IRemoteNuget NugetFor(Solution solution, Dependency dependency);
		IEnumerable<IRemoteNuget> UpdatesFor(Solution solution);

		IEnumerable<IRemoteNuget> DependenciesFor(Solution solution, Dependency dependency);
	}

	public class FeedService : IFeedService
	{
		public IRemoteNuget NugetFor(Solution solution, Dependency dependency)
		{
			IRemoteNuget nuget = null;
			var feeds = solution.Feeds.Select(x => x.GetNugetFeed());
			foreach (var feed in feeds)
			{
				nuget = getLatestFromFloatingFeed(feed, dependency);
				if (nuget != null) break;

				if (dependency.IsFloat())
				{
					nuget = feed.FindLatest(dependency);
					if (nuget != null) break;
				}

				nuget = feed.Find(dependency);
				if (nuget != null) break;
			}

			if (nuget == null)
			{
				throw new ArgumentOutOfRangeException("dependency", "Could not find " + dependency);
			}

			return remoteOrCached(solution, nuget);
		}

		private IRemoteNuget remoteOrCached(Solution solution, IRemoteNuget nuget)
		{
			if (nuget == null) return null;
			return solution.Cache.Retrieve(nuget);
		}

		private IRemoteNuget getLatestFromFloatingFeed(INugetFeed feed, Dependency dependency)
		{
			var floatingFeed = feed as IFloatingFeed;
			if (floatingFeed == null) return null;

			var floatedResult = floatingFeed.GetLatest().SingleOrDefault(x => x.Name == dependency.Name);
			if (floatedResult != null && dependency.Mode == UpdateMode.Fixed && floatedResult.IsUpdateFor(dependency))
			{
				return null;
			}

			return floatedResult;
		}

		public IEnumerable<IRemoteNuget> UpdatesFor(Solution solution)
		{
			var nugets = new List<IRemoteNuget>();
			var tasks = solution.Dependencies.Select(dependency => updateNuget(nugets, solution, dependency)).ToArray();

			Task.WaitAll(tasks);

			return nugets;
		}

		public IEnumerable<IRemoteNuget> DependenciesFor(Solution solution, Dependency dependency)
		{
			var nuget = NugetFor(solution, dependency);
			var feeds = solution.Feeds.Select(x => x.GetNugetFeed()).ToList();
			var dependencies = new List<IRemoteNuget>();

			foreach (var feed in feeds)
			{
				var package = feed.Repository.FindPackage(nuget.Name, nuget.Version);
				if (package == null) continue;

				var dependents = package
					.DependencySets
					.SelectMany(x => x.Dependencies);

				dependents.Each(x =>
				{
					var version = x.VersionSpec.MaxVersion ?? x.VersionSpec.MinVersion;
					var dependentNuget = NugetFor(solution, new Dependency(x.Id, version.ToString()));
					dependencies.Add(dependentNuget);
				});
			}
			
			return dependencies;
		}

		private Task updateNuget(List<IRemoteNuget> nugets, Solution solution, Dependency dependency)
		{
			return Task.Factory.StartNew(() => LatestFor(solution, dependency).CallIfNotNull(nugets.Add));
		}

		public IRemoteNuget LatestFor(Solution solution, Dependency dependency)
		{
			if (dependency.Mode == UpdateMode.Fixed)
			{
				return null;
			}

			IRemoteNuget latest = null;
			var feeds = solution.Feeds.Select(x => x.GetNugetFeed());

			foreach (var feed in feeds)
			{
				try
				{
					var nuget = feed.FindLatest(dependency);
					if (latest == null)
					{
						latest = nuget;
					}

					if (latest != null && latest.Version < nuget.Version)
					{
						latest = nuget;
					}
				}
				catch (Exception exc)
				{
					RippleLog.Error("Error while finding latest " + dependency, exc);
				}
			}

			if (isUpdate(latest, solution, dependency))
			{
				return remoteOrCached(solution, latest);
			}

			return null;
		}

		private bool isUpdate(IRemoteNuget latest, Solution solution, Dependency dependency)
		{
			if (latest == null) return false;

			var localDependencies = solution.LocalDependencies();
			if (localDependencies.Has(dependency))
			{
				var local = localDependencies.Get(dependency);
				return latest.IsUpdateFor(local);
			}

			if (dependency.IsFloat())
			{
				return true;
			}

			return latest.IsUpdateFor(dependency);
		}
	}
}