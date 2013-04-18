using ripple.Model;

namespace ripple.Nuget
{
    public interface INugetStepRunner
    {
        void AddSolutionDependency(Dependency dependency);
        void AddProjectDependency(string project, Dependency dependency);

        void RemoveDependency(Dependency dependency);
        void UpdateDependency(Dependency dependency);
    }
}