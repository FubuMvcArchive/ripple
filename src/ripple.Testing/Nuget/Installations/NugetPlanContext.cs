using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget.Installations
{
    public class NugetPlanContext
    {
         public INugetStep projectInstallation(string project, string name)
         {
             return new InstallProjectDependency(project, new Dependency(name) { Mode = UpdateMode.Float });
         }

        public INugetStep solutionInstallation(string name, string version, UpdateMode mode)
        {
            return new InstallSolutionDependency(new Dependency(name, version, mode));
        }

        public INugetStep updateSolutionDependency(string name, string version, UpdateMode mode)
        {
            return null;
        }
    }
}