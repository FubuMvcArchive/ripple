using ripple.Model;

namespace ripple.Nuget
{
    public interface INugetStepRunner
    {
        void AddSolutionDependency(Dependency dependency);
        void AddProjectDependency(string project, Dependency dependency);

        void UpdateDependency(Dependency dependency);
    }

    public class NugetStepRunner : INugetStepRunner
    {
        private readonly Solution _solution;

        public NugetStepRunner(Solution solution)
        {
            _solution = solution;
        }

        public void AddSolutionDependency(Dependency dependency)
        {
            _solution.AddDependency(dependency);
        }

        public void AddProjectDependency(string project, Dependency dependency)
        {
            _solution.FindProject(project).AddDependency(dependency.AsFloat());
        }

        public void UpdateDependency(Dependency dependency)
        {
            var existing = _solution.Dependencies.Find(dependency.Name);
            dependency.Mode = existing.Mode;

            _solution.UpdateDependency(dependency);
        }
    }
}