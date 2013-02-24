using System.Collections.Generic;

namespace ripple.New.Nuget
{
    public interface IFloatingFeed : INugetFeed
    {
        IEnumerable<IRemoteNuget> GetLatest();
    }
}