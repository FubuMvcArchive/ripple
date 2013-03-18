using System.Collections.Generic;
using NuGet;
using ripple.New.Nuget;

namespace ripple.New.Model
{
	public interface IFeedService
	{
		IRemoteNuget NugetFor(Solution solution, Dependency dependency);
		IEnumerable<IRemoteNuget> UpdatesFor(Solution solution);

		IEnumerable<PackageDependency> DependenciesFor(Solution solution, Dependency dependency);
	}
}