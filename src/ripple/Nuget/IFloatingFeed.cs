using System.Collections.Generic;

namespace ripple.Nuget
{
    public interface IFloatingFeed : INugetFeed
    {
        IEnumerable<IRemoteNuget> GetLatest();
    }
}