using System.Collections.Generic;
using ripple.Model;

namespace ripple.Nuget
{
    public interface INugetOperationContext
    {
        IEnumerable<NugetPlanRequest> Requests(Solution solution);
    }
}