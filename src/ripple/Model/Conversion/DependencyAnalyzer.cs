using System.Collections.Generic;
using System.Linq;

namespace ripple.Model.Conversion
{
    public class DependencyAnalyzer
    {
        private readonly IList<Dependency> _dependencies = new List<Dependency>();

        public void Analyze(Project project, NuGetConversionReport report)
        {
            var dependencies = FixedDependenciesFor(project);
            dependencies.Each(dependency =>
            {
                var existing = _dependencies.SingleOrDefault(x => x.MatchesName(dependency));
                if (existing == null)
                {
                    _dependencies.Add(dependency.Copy());
                }
                else if (dependency.SemanticVersion() > existing.SemanticVersion())
                {
                    // TODO -- Report a warning
                    _dependencies.Remove(existing);
                    _dependencies.Add(dependency.Copy());
                }

                dependency.Float();
            });
        }

        public void Fill(Solution solution)
        {
            _dependencies.Each(solution.AddDependency);
        }

        public IEnumerable<Dependency> FixedDependenciesFor(Project project)
        {
            return project.Dependencies.Where(x => x.IsFixed());
        }
    }
}