using System.Diagnostics;
using System.IO;
using FubuCore.Util;
using NuGet;
using System.Linq;
using NuGet.Common;
using ripple.Local;
using ripple.Model;
using System.Collections.Generic;
using Console = System.Console;

namespace ripple.MSBuild
{
    public class ReferenceAttacher
    {
        private readonly Solution _solution;
        private readonly LocalPackageRepository _repository;
        private readonly Cache<string, IPackage> _packages;

        public ReferenceAttacher(Solution solution)
        {
            _solution = solution;
            _repository = new LocalPackageRepository(_solution.PackagesFolder());

            _packages = new Cache<string, IPackage>(name => _repository.FindPackage(name));
        }

        public void Attach()
        {
            _solution.Projects.Each(fixProject);
        }

        private void fixProject(Project project)
        {
            project = Project.ReadFrom(project.ProjectFile);
            var file = new CsProjFile(project.ProjectFile);
            bool needsSaved = false;

            project.NugetDependencies.Each(dep => {
                var package = _packages[dep.Name];
                if (package == null)
                {
                    Console.WriteLine("Could not find the IPackage for " + dep.Name);
                    return;
                }

                var assemblies = package.AssemblyReferences;
                if (assemblies == null) return;

                assemblies.Each(assem => {
                    var hintPath = Path.Combine("..", "packages", dep.ToNugetFolderName(), assem.Path);
                    var assemblyName = Path.GetFileNameWithoutExtension(assem.Name);

                    if (file.AddReference(assemblyName, hintPath) == ReferenceStatus.Changed)
                    {
                        Console.WriteLine("Updated reference for {0} to {1}", project.ProjectFile, hintPath);
                        needsSaved = true;
                    }
                });
            });

            if (needsSaved)
            {
                Console.WriteLine("Writing changes to " + file);
                file.Write();
            }
        }   
    }
}