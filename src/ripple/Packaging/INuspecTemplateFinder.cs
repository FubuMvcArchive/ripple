using System.Collections.Generic;
using System.Linq;
using FubuCore;
using ripple.Model;

namespace ripple.Packaging
{
    public interface INuspecTemplateFinder
    {
        NuspecTemplateCollection Templates(Solution solution);
    }

    public class NuspecTemplateFinder : INuspecTemplateFinder
    {
        public NuspecTemplateCollection Templates(Solution solution)
        {
            var specs = new List<ProjectNuspec>();
            solution.EachProject(project =>
            {
                solution.Specifications.Each(spec =>
                {
                    if (spec.Name.EqualsIgnoreCase(project.Name))
                    {
                        specs.Add(new ProjectNuspec(project, spec));
                    }
                });
            });

            specs.AddRange(solution.Nuspecs.Select(x => x.ToSpec(solution)));

            return new NuspecTemplateCollection(specs
                .GroupBy(x => x.Publishes)
                .Select(x => new NuspecTemplate(x.Key, x)));
        }
    }
}