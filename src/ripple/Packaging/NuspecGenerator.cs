using System.Collections.Generic;
using System.Linq;
using NuGet;
using ripple.Model;

namespace ripple.Packaging
{
    public interface INuspecGenerator
    {
        NuspecGenerationPlan PlanFor(Solution solution, SemanticVersion version);
    }

    public class NuspecGenerator : INuspecGenerator
    {
        private readonly INuspecTemplateFinder _finder;
        private readonly IEnumerable<INuspecDependencySource> _sources;

        public NuspecGenerator(INuspecTemplateFinder finder, IEnumerable<INuspecDependencySource> sources)
        {
            _finder = finder;
            _sources = sources;
        }

        public NuspecGenerationPlan PlanFor(Solution solution, SemanticVersion version)
        {
            var plan = new NuspecGenerationPlan(solution, version);
            var templates = _finder.Templates(solution);

            templates.Each(template =>
            {
                var child = new NuspecPlan(template, version);
                var context = new NuspecTemplateContext(template, templates, solution, version);

                child.AddDependencies(_sources.SelectMany(x => x.DetermineDependencies(context)));

                plan.Add(child);
            });

            return plan;
        }

        public static NuspecGenerator Basic()
        {
            return new NuspecGenerator(new NuspecTemplateFinder(), new INuspecDependencySource[]
            {
                new DirectDependenciesSource(),
                new ProjectDependenciesSource(),
                new NugetSpecDependenciesSource()
            });
        }
    }
}