using System.Collections.Generic;
using ripple.Nuget;

namespace ripple.Model
{
	public interface IFeedService
	{
		IRemoteNuget NugetFor(Solution solution, Dependency dependency);

		IEnumerable<Dependency> DependenciesFor(Solution solution, Dependency dependency, UpdateMode mode);
	}
}