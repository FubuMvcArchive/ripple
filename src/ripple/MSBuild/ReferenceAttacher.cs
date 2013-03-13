using FubuCore.Util;
using NuGet;
using System.Collections.Generic;
using ripple.New.Model;
using Console = System.Console;
using Project = ripple.Local.Project;
using Solution = ripple.Model.Solution;

namespace ripple.MSBuild
{
    public class ReferenceAttacher
    {
        private readonly Solution _solution;
        private readonly LocalPackageRepository _repository;
        private readonly Cache<string, IPackage> _packages;
        private readonly Cache<string, string> _toolsVersionMatch = new Cache<string, string>();

        public ReferenceAttacher(Solution solution)
        {
            _solution = solution;
            _repository = new LocalPackageRepository(_solution.PackagesFolder());

            _packages = new Cache<string, IPackage>(name => _repository.FindPackage(name));
            _toolsVersionMatch["4.0"] = "net40";
        }

        public void Attach()
        {
            _solution.Projects.Each(fixProject);
        }

        private void fixProject(Project project)
        {
            project = Project.ReadFrom(project.ProjectFile);
            var file = new CsProjFile(project.ProjectFile);
            

            project.NugetDependencies.Each(dep => {
                var package = _packages[dep.Name];
                if (package == null)
                {
                    Console.WriteLine("Could not find the IPackage for " + dep.Name);
                    return;
                }

                var assemblies = package.AssemblyReferences;
                if (assemblies == null) return;

                file.AddAssemblies(new Dependency(dep.Name, dep.Version), assemblies);


            });


        }   
    }
}