using System;
using System.Collections.Generic;
using System.Diagnostics;
using FubuCore;
using System.Linq;
using FubuCore.CommandLine;
using ripple.Local;

namespace ripple.Model
{
    public enum CleanMode
    {
        all,
        packages,
        projects
    }

    public class Solution
    {
        public static Solution ReadFrom(string directory)
        {
            try
            {
                var config = SolutionConfig.LoadFrom(directory);

                var solution = new Solution(config, directory);

                var system = new FileSystem();
                readProjects(directory, system, solution);


                readNugetSpecs(directory, config, system, solution);

                return solution;
            }
            catch (Exception)
            {
                Console.WriteLine("Error reading Solution from " + directory);
                throw;
            }
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
            _directory = directory.ToFullPath();
        }

        public string Directory
        {
            get { return _directory; }
        }

        public string Name
        {
            get
            {
                return _config.Name;
            }
        }

        public SolutionConfig Config
        {
            get { return _config; }
        }

        public void Clean(IFileSystem fileSystem, CleanMode mode)
        {
            if (mode == CleanMode.all || mode == CleanMode.packages)
            {
                var packagesFolder = PackagesFolder();
                Console.WriteLine("Deleting " + packagesFolder);
                fileSystem.DeleteDirectory(packagesFolder);
            }

            if (mode == CleanMode.all || mode == CleanMode.projects)
            {
                _projects.Each(p => p.Clean(fileSystem));    
            }

            
        }

        public string PackagesFolder()
        {
            return _directory.AppendPath(_config.SourceFolder, "packages");
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
            IEnumerable<NugetDependency> nugetDependencies = GetAllNugetDependencies();

            nugetDependencies.Each(x =>
            {
                var spec = finder(x.Name);
                if (spec != null)
                {
                    _dependencies.Add(spec);
                }
            });
        }

        public IEnumerable<NugetDependency> GetAllNugetDependencies()
        {
            return Projects.SelectMany(x => x.NugetDependencies).Distinct();
        }

        public IEnumerable<NugetSpec> NugetDependencies()
        {
            return _dependencies;
        }

        public IEnumerable<Solution> SolutionDependencies()
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

        public void AddProject(Project project)
        {
            _projects.Add(project);
        }

        public string NugetFolderFor(NugetSpec spec)
        {
            var dependency = Projects.SelectMany(x => x.NugetDependencies).Distinct()
                .Single(x => x.Name == spec.Name);

            return _directory.AppendPath(_config.SourceFolder, "packages", spec.Name + "." + dependency.Version);
        }

        public ProcessStartInfo CreateBuildProcess(bool fast)
        {
            var cmdLine = fast ? _config.FastBuildCommand : _config.BuildCommand;
            var commands = StringTokenizer.Tokenize(cmdLine);

            return new ProcessStartInfo(commands.First()){
                WorkingDirectory = _directory,
                Arguments = commands.Skip(1).Join(" ")
            };
        }
    }
}