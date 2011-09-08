using System;
using System.Collections.Generic;
using FubuCore;
using System.Linq;

namespace ripple
{
    public class Solution
    {
        public static Solution ReadFrom(string directory)
        {
            var config = SolutionConfig.LoadFrom(directory);

            var solution = new Solution(config, directory);

            var system = new FileSystem();
            readProjects(directory, system, solution);


            readNugetSpecs(directory, config, system, solution);

            return solution;
        }

        private static void readNugetSpecs(string directory, SolutionConfig config, FileSystem system, Solution solution)
        {
            system.FindFiles(directory.AppendPath(config.NugetSpecFolder), new FileSet(){
                Include = "*.nuspec"
            })
                .Each(file =>
                {
                    var spec = NugetSpec.ReadFrom(file);
                    solution.AddNugetSpec(spec);
                });
        }

        private static void readProjects(string directory, FileSystem system, Solution solution)
        {
            system.FindFiles(directory, new FileSet(){
                Include = "packages.config"
            }).Each(file =>
            {
                var project = Project.ReadFrom(file);
                solution._projects.Add(project);
            });
        }

        private readonly SolutionConfig _config;
        private readonly string _directory;
        private readonly IList<Project> _projects = new List<Project>();
        private readonly IList<NugetSpec> _published = new List<NugetSpec>();
        private readonly IList<NugetSpec> _dependencies = new List<NugetSpec>();

        public Solution(SolutionConfig config, string directory)
        {
            _config = config;
            _directory = directory;
        }

        public string Name
        {
            get
            {
                return _config.Name;
            }
        }

        public void Clean(IFileSystem fileSystem)
        {
            var packagesFolder = _directory.AppendPath(_config.SourceFolder, "packages");
            fileSystem.DeleteDirectory(packagesFolder);

            _projects.Each(p => p.Clean(fileSystem));
        }

        public void AddNugetSpec(NugetSpec spec)
        {
            _published.Add(spec);
            spec.Publisher = this;
        }

        public IEnumerable<Project> Projects
        {
            get { return _projects; }
        }

        public Project FindProject(string name)
        {
            return _projects.FirstOrDefault(x => x.ProjectName == name);
        }

        public IEnumerable<NugetSpec> PublishedNugets
        {
            get
            {
                return _published;
            }
        }

        public void DetermineDependencies(Func<string, NugetSpec> finder)
        {
            var nugetDependencies = Projects.SelectMany(x => x.NugetDependencies).Distinct();
            
            nugetDependencies.Each(x =>
            {
                var spec = finder(x.Name);
                if (spec != null)
                {
                    _dependencies.Add(spec);
                }
            });
        }

        public IEnumerable<Solution> Dependencies()
        {
            return _dependencies.Select(x => x.Publisher)
                .Distinct()
                .OrderBy(x => x.Name);
        }

        public bool DependsOn(Solution peer)
        {
            return _dependencies.Any(x => x.Publisher == peer);
        }

        public override string ToString()
        {
            return string.Format("Solution {0}", Name);
        }
    }
}