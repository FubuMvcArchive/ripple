using System.Collections.Generic;
using System.Linq;
using ripple.Model;

namespace ripple.Packaging
{
    public class ProjectDependenciesSource : INuspecDependencySource
    {
        public IEnumerable<NuspecDependencyToken> DetermineDependencies(NuspecTemplateContext context)
        {
            var projects = context
                .Current
                .Projects
                .Where(x => x.References.Any())
                .SelectMany(x => x.References);

            var tokens = new List<NuspecDependencyToken>();
            projects.Each(projectRef =>
            {
                var target = context
                    .Templates
                    .Except(context.Current)
                    .FindByProject(projectRef);

                if (target == null) return;

                var constraint = context.Solution.NuspecSettings.Float;
                tokens.Add(new NuspecDependencyToken(new Dependency(projectRef.Name), context.Version, constraint));
            });

            return tokens;
        }
    }

    public class NugetSpecDependenciesSource : INuspecDependencySource
    {
        public IEnumerable<NuspecDependencyToken> DetermineDependencies(NuspecTemplateContext context)
        {
            return context
                .Current
                .NugetSpecDependencies()
                .Select(spec =>
                {
                    var constraint = context.Solution.NuspecSettings.Float;
                    return new NuspecDependencyToken(spec.Name, context.Version, constraint);
                });
        }
    }
}