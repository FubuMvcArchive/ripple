using System;

namespace ripple.Nuget
{
    public interface INugetPlanBuilder
    {
        NugetPlan PlanFor(NugetPlanRequest request);
    }

    public class NugetPlanBuilder : INugetPlanBuilder
    {
        // Going to let the update command make multiple calls and aggregate the plans
        public NugetPlan PlanFor(NugetPlanRequest request)
        {
            return new NugetPlan();
        }
    }
}