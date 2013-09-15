using System.Collections.Generic;
using System.Threading.Tasks;
using ripple.Nuget;

namespace ripple.Model
{
    public enum SearchLocation
    {
        Local,
        Remote
    }

    public interface IFeedService
    {
        Task<NugetResult> NugetFor(Dependency dependency);

        IEnumerable<Dependency> DependenciesFor(Dependency dependency, UpdateMode mode, SearchLocation location = SearchLocation.Remote);
    }
}