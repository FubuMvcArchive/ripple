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
                readProjects(config.GetSolutionFolder(directory), system, solution);
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
            var csProjSet = new FileSet(){
                Include = "*.csproj"
            };

            system.FindFiles(directory, csProjSet).Each(file =>
            {
                var project = Project.ReadFrom(file);

                solution.AddProject(project);
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

        public void AlterConfig(Action<SolutionConfig> alteration)
        {
            alteration(_config);

            var file = _directory.AppendPath(SolutionConfig.FileName);
            Console.WriteLine("Writing changes to " + file);
            new FileSystem().PersistToFile(_config, file);
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

        public NugetDependency GetLatestNugetOf(string nugetName)
        {
            return GetAllNugetDependencies().OrderByDescending(x => x.Version).FirstOrDefault(x => x.Name == nugetName);
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
            project.NugetDependencies.Each(dep =>
            {
                dep.UpdateMode = _config.ModeForNuget(dep.Name);
            });
        }

        public string NugetFolderFor(NugetSpec spec)
        {
            var nugetName = spec.Name;

            return NugetFolderFor(nugetName);
        }

        public string NugetFolderFor(string nugetName)
        {
            var nugetDependencies = Projects.SelectMany(x => x.NugetDependencies).Distinct();
            NugetDependency dependency = null;
            
            try
            {
                dependency = nugetDependencies
                    .Single(x => x.Name.EqualsIgnoreCase(nugetName));
            }
            catch (InvalidOperationException ex)
            {
                var options = nugetDependencies.Select(d=>d.Name).Aggregate((l,r)=> l+", "+r);
                throw new InvalidOperationException(string.Format("Couldn't select a single dependency for '{0}'. Couldn't decide between {1}./nTry running 'ripple update'.", nugetName, options));
            }

            return _directory.AppendPath(_config.SourceFolder, "packages", nugetName + "." + dependency.Version);
        }

        public ProcessStartInfo CreateBuildProcess(bool fast)
        {
            var cmdLine = fast ? _config.FastBuildCommand : _config.BuildCommand;
            var commands = StringTokenizer.Tokenize(cmdLine);

            var fileName = commands.First();
            if (fileName == "rake")
            {
                fileName = RippleFileSystem.RakeRunnerFile();
            }

            return new ProcessStartInfo(fileName){
                WorkingDirectory = _directory,
                Arguments = commands.Skip(1).Join(" ")
            };
        }
    }
}