using System.Collections.Generic;
using NuGet;
using ripple.Nuget;

namespace ripple.Model
{
	public interface IFeedService
	{
		IRemoteNuget NugetFor(Solution solution, Dependency dependency);
		IEnumerable<IRemoteNuget> UpdatesFor(Solution solution);
		IRemoteNuget UpdateFor(Solution solution, Dependency dependency);

		IEnumerable<PackageDependency> DependenciesFor(Solution solution, Dependency dependency);
	}
}