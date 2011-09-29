using System.Collections.Generic;
using ripple.Local;

namespace ripple.Nuget
{
    public interface INugetService
    {
        NugetDependency GetLatest(string nugetName);

        void Install(NugetDependency dependency);

        void RemoveFromFileSystem(NugetDependency dependency);
        void Update(Project project, IEnumerable<NugetDependency> dependencies);
        void RemoveFromProject(Project project, IEnumerable<NugetDependency> dependencies);
    }
}