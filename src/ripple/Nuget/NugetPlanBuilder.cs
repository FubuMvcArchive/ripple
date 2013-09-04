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
        private readonly Cache<PlanKey, NugetPlan> _planCache;

        public NugetPlanBuilder()
        {
            _planCache = new Cache<PlanKey, NugetPlan>();
        }

        public NugetPlan PlanFor(NugetPlanRequest request)
        {
            return buildPlan(request);
        }

        private NugetPlan buildPlan(NugetPlanRequest request, Dependency parent = null)
        {
            var plan = new NugetPlan();
            var target = request.Dependency;
            var solution = request.Solution;

            var dependencyKey = target.Copy();
            if (dependencyKey.IsFloat())
            {
                dependencyKey = target.AsFloat();
            }

            var key = new PlanKey(dependencyKey, request.Project);
            if (_planCache.Has(key))
            {
                return _planCache[key];
            }

            _planCache.Fill(key, plan);

            RippleLog.Info("* Analyzing " + target);

            if (target.Version.IsEmpty())
            {
                string version = null;
                var local = solution.LocalDependencies();

                if (request.Operation == OperationType.Install && solution.LocalDependencies().Has(target))
                {
                    var localNuget = local.Get(target);
                    version = localNuget.Version.ToString();
                }
                else
                {
                    if (!RippleEnvironment.Connected())
                    {
                        RippleAssert.Fail("Cannot update in offline mode");
                    }

                    var task = solution.FeedService.NugetFor(target);
                    task.Wait();

                    if (!task.Result.Found)
                    {
                        RippleAssert.Fail("Could not find " + request.Dependency);
                    }
                    var remote = task.Result.Nuget;
                    version = remote.Version.ToString();
                }

                target.Version = version;
            }

            if (request.UpdatesCurrentDependency())
            {
                updateDependency(plan, request);
            }
            else if (!solution.Dependencies.Has(target.Name))
            {
                plan.AddStep(new InstallSolutionDependency(target));
            }

            projectInstallations(plan, parent, request);

            var location = request.Operation == OperationType.Install ? SearchLocation.Local : SearchLocation.Remote;

            var nugetDependencies = solution.FeedService.DependenciesFor(target, target.Mode, location);
            nugetDependencies.Each(x =>
            {
                var transitiveDep = request.CopyFor(x);
                var childPlan = buildPlan(transitiveDep, target);
                plan.Import(childPlan);
            });

            return plan;
        }

        private void updateDependency(NugetPlan plan, NugetPlanRequest request)
        {
            var target = request.Dependency;
            var solution = request.Solution;

            var configured = solution.Dependencies.Find(target.Name);

            if (!request.ShouldUpdate(configured))
            {
                RippleLog.Info("Warning: This operation requires {0} to be updated to {1} but it is marked as fixed. Use the force option to correct this.".ToFormat(target.Name, target.Version));
                return;
            }

            plan.AddStep(new UpdateDependency(target));
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

        public class PlanKey
        {
            public PlanKey(Dependency dependency, string project)
            {
                Dependency = dependency;
                Project = project;
            }

            public Dependency Dependency { get; private set; }
            public string Project { get; private set; }

            protected bool Equals(PlanKey other)
            {
                return Equals(Dependency, other.Dependency) && string.Equals(Project, other.Project);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((PlanKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Dependency != null ? Dependency.GetHashCode() : 0) * 397) ^ (Project != null ? Project.GetHashCode() : 0);
                }
            }
        }
    }
}