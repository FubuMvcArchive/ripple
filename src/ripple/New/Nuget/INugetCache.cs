using System.Collections.Generic;
using ripple.New.Model;

namespace ripple.New.Nuget
{
    public interface INugetCache
    {
        void UpdateAll(IEnumerable<IRemoteNuget> nugets);
		INugetFile Latest(Dependency query);

        void Flush();

		INugetFile Find(Dependency query);
    }
}