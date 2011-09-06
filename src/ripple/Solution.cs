using System;
using System.Collections.Generic;
using FubuCore;

namespace ripple
{
    public class Solution
    {
        private readonly SolutionConfig _config;
        private readonly string _directory;
        private readonly IList<Project> _projects = new List<Project>();
        private readonly IList<NugetSpec> _nugets = new List<NugetSpec>();


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
            _nugets.Add(spec);
            spec.Publisher = this;
        }

        public IEnumerable<NugetSpec> PublishedNugets
        {
            get
            {
                return _nugets;
            }
        }

        public IEnumerable<SolutionDependency> FindImmediateDependencies(Func<string, NugetSpec> nugetSource)
        {
            throw new NotImplementedException();
        }
    }
}