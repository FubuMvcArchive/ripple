using ripple.Model;

namespace ripple.Nuget
{
    // Probably going to make this guy "transactional"
    public interface INugetStepRunner
    {
        void AddSolutionDependency(Dependency dependency);
        void AddProjectDependency(string project, Dependency dependency);

        void UpdateSolutionDependency(Dependency dependency);

        void RemoveDependency(Dependency dependency);
        void UpdateDependency(Dependency dependency);
    }
}