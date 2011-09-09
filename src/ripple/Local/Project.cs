using System.Collections.Generic;
using System.IO;
using FubuCore;
using System.Linq;

namespace ripple.Local
{
    public class Project
    {
        private readonly IList<NugetDependency> _nugetDependencies = new List<NugetDependency>();
        public static readonly string PackagesConfig = "packages.config";
        private readonly string _directory;
        private readonly string _projectName;

        public static Project ReadFrom(string file)
        {
            var project = new Project(file);
            project._nugetDependencies.AddRange(NugetDependency.ReadFrom(file));

            return project;
        }

        public Project(string filename)
        {
            _directory = Path.GetDirectoryName(filename);
            _projectName = _directory.Split(Path.DirectorySeparatorChar).Last();
        }

        public string PackagesFile()
        {
            return _directory.AppendPath("packages.config");
        }

        public string ProjectName
        {
            get
            {
                return _projectName;
            }
        }

        public void AddDependency(NugetDependency dependency)
        {
            _nugetDependencies.Add(dependency);
        }

        public IEnumerable<NugetDependency> NugetDependencies
        {
            get { return _nugetDependencies; }
        }

        public void Clean(IFileSystem system)
        {
            system.CleanWithTracing(_directory.AppendPath("bin"));
            system.CleanWithTracing(_directory.AppendPath("obj"));
        }
    }
}