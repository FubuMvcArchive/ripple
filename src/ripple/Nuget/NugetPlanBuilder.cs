using System.Collections.Generic;
using ripple.Model;

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
            if (request.Operation == OperationType.Install)
            {
                return InstallFor(request);
            }

            return UpdateFor(request);
        }

        public NugetPlan UpdateFor(NugetPlanRequest request)
        {
            return new NugetPlan();
        }

        public NugetPlan InstallFor(NugetPlanRequest request)
        {
            var plan = new NugetPlan();
            var target = request.Dependency;
            var solution = request.Solution;

            if (solution.Dependencies.Has(target.Name) || solution.LocalDependencies().Has(target))
            {
                return plan;
            }

            plan.AddStep(new InstallSolutionDependency(target));
            
            if (request.InstallToProject())
            {
                plan.AddStep(new InstallProjectDependency(request.Project, Dependency.FloatFor(target.Name)));
            }

            var nugetDependencies = solution.FeedService.DependenciesFor(solution, target, target.Mode);
            nugetDependencies.Each(x =>
            {
                var childPlan = InstallFor(request.CopyFor(x));
                plan.Import(childPlan);
            });

            return plan;
        }
    }
}