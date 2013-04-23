using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using ripple.Model;

namespace ripple.Nuget
{
    public interface INugetPlanBuilder
    {
        NugetPlan PlanFor(NugetPlanRequest request);
    }

    public class NugetPlanBuilder : INugetPlanBuilder
    {
        private readonly Cache<Dependency, NugetPlan> _planCache;

        public NugetPlanBuilder()
        {
            _planCache = new Cache<Dependency, NugetPlan>();
        }

        public NugetPlan PlanFor(NugetPlanRequest request)
        {
            return buildPlan(request);
        }

        private NugetPlan buildPlan(NugetPlanRequest request, Dependency parent = null, int depth = 0)
        {
            var plan = new NugetPlan();
            var target = request.Dependency;
            var solution = request.Solution;

            var key = target.Copy();
            if (key.IsFloat())
            {
                key = target.AsFloat();
            }

            if (_planCache.Has(key))
            {
                return _planCache[key];
            }

            _planCache.Fill(key, plan);

            RippleLog.Info("* Analyzing " + target);

            if (target.Version.IsEmpty())
            {
                var remote = solution.FeedService.NugetFor(solution, target);
                target.Version = remote.Version.ToString();
            }

            if (request.UpdatesCurrentDependency())
            {
                var configured = solution.Dependencies.Find(target.Name);
                var shouldUpdate = (depth != 0 || request.Operation == OperationType.Update) && (request.ForceUpdates || configured.IsFloat());
                if (shouldUpdate)
                {
                    plan.AddStep(new UpdateDependency(target));
                }
                else
                {
                    RippleLog.Info("Warning: This operation requires {0} to be updated to {1} but it is marked as fixed. Use the force option to correct this.".ToFormat(target.Name, target.Version));
                }
            }
            else if(!solution.Dependencies.Has(target.Name))
            {
                plan.AddStep(new InstallSolutionDependency(target));
            }

            projectInstallations(plan, parent, request);

            var nugetDependencies = solution.FeedService.DependenciesFor(solution, target, target.Mode);
            nugetDependencies.Each(x =>
            {
                var childPlan = buildPlan(request.CopyFor(x), target, depth + 1);
                plan.Import(childPlan);
            });

            return plan;
        }

        private void projectInstallations(NugetPlan plan, Dependency parent, NugetPlanRequest request)
        {
            var target = request.Dependency;
            var solution = request.Solution;

            if (request.InstallToProject())
            {
                var project = solution.FindProject(request.Project);
                installToProject(plan, project, target);
            }

            if (parent == null) return;

            var projects = solution.Projects.Where(x => x.Dependencies.Has(parent.Name));
            projects.Each(project => installToProject(plan, project, target));
        }

        private void installToProject(NugetPlan plan, Project project, Dependency target)
        {
            if (!project.Dependencies.Has(target.Name))
            {
                plan.AddStep(new InstallProjectDependency(project.Name, Dependency.FloatFor(target.Name)));
            }
        }
    }
}