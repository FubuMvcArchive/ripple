using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using ripple.Model.Conversion;
using ripple.Model.Xml;

namespace ripple.Model
{
    public class SolutionFiles : ISolutionFiles
    {
        public const string ConfigFile = "ripple.config";

        private static readonly IList<ISolutionLoader> Loaders = new List<ISolutionLoader>();
 
        static SolutionFiles()
        {
            Reset();
        }

        public static void Reset()
        {
            Loaders.Clear();
            
            AddLoader(new XmlSolutionLoader());
            AddLoader(new NuGetSolutionLoader());
        }

        public static void AddLoader(ISolutionLoader loader)
        {
            Loaders.Add(loader);
        }

        private readonly IFileSystem _fileSystem;
        private readonly Lazy<ISolutionLoader> _loader;
        private Func<ISolutionLoader> _findLoader; 

        public SolutionFiles(IFileSystem fileSystem)
        {
            if (RippleFileSystem.IsSolutionDirectory())
            {
                resetDirectories(RippleFileSystem.FindSolutionDirectory());
            }

            _fileSystem = fileSystem;
            _findLoader = findLoader;
            _loader = new Lazy<ISolutionLoader>(() => _findLoader());
        }

        private void resetDirectories(string root)
        {
            RootDir = root;
        }

        public ISolutionLoader Loader
        {
            get { return _loader.Value; }
        }

        private ISolutionLoader findLoader()
        {
            var solutionLoader = Loaders.FirstOrDefault(x => x.Condition.Matches(_fileSystem, RootDir));
            if (solutionLoader == null)
            {
                RippleAssert.Fail("Unable to determine ripple mode. See the log for further details or use the --verbose option.");
            }

            return solutionLoader;
        }

        public string RootDir { get; set; }

        public void ForProjects(Solution solution, Action<string> action)
        {
            var projSet = new FileSet { Include = "*.csproj;*.vbproj;*.fsproj" };
            var targetDir = Path.Combine(solution.Directory, solution.SourceFolder);

            _fileSystem.FindFiles(targetDir, projSet).Each(action);
        }

        public void ForNuspecs(Solution solution, Action<string> action)
        {
            var nuspecSet = new FileSet { Include = "*.nuspec" };
            var targetDir = Path.Combine(solution.Directory, solution.NugetSpecFolder);

            _fileSystem.FindFiles(targetDir, nuspecSet).Each(action);
        }

        public Solution LoadSolution()
        {
            var file = Path.Combine(RootDir, ConfigFile);

            var solution = Loader.LoadFrom(_fileSystem, file);
            solution.Path = file;

            return solution;
        }

        public void FinalizeSolution(Solution solution)
        {
            Loader.SolutionLoaded(solution);
        }

        public static SolutionFiles Basic()
        {
            return new SolutionFiles(new FileSystem());
        }

        public static SolutionFiles FromDirectory(string directory)
        {
            var files = Basic();
            files.resetDirectories(directory);

            return files;
        }

        public static SolutionFiles FromDirectory(string directory, ISolutionLoader loader)
        {
            var files = new SolutionFiles(new FileSystem());
            files.resetDirectories(directory);
            files._findLoader = () => loader;

            return files;
        }
    }
}