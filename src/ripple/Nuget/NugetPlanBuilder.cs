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

        public NugetPlan InstallFor(NugetPlanRequest request, int depth = 0)
        {
            var plan = new NugetPlan();
            var target = request.Dependency;
            var solution = request.Solution;

            if (target.IsFloat())
            {
                var remote = solution.FeedService.NugetFor(solution, target);
                target.Version = remote.Version.ToString();
            }

            
            if (request.UpdatesCurrentDependency())
            {
                var shouldUpdate = (depth != 0) && request.ForceUpdates;
                if(shouldUpdate) plan.AddStep(new UpdateDependency(target));
            }
            else
            {
                plan.AddStep(new InstallSolutionDependency(target));
            }
            
            if (request.InstallToProject())
            {
                var project = solution.FindProject(request.Project);
                if (!project.Dependencies.Has(target.Name))
                {
                    plan.AddStep(new InstallProjectDependency(request.Project, Dependency.FloatFor(target.Name)));
                }
            }

            var nugetDependencies = solution.FeedService.DependenciesFor(solution, target, target.Mode);
            nugetDependencies.Each(x =>
            {
                var childPlan = InstallFor(request.CopyFor(x), depth + 1);
                plan.Import(childPlan);
            });

            return plan;
        }
    }
}