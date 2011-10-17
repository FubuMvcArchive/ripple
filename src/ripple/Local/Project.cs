using System;
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
            file = file.ToFullPath();
            var project = new Project(file);
            project._nugetDependencies.AddRange(NugetDependency.ReadFrom(file));

            var files = new FileSystem().FindFiles(file.ParentDirectory(), new FileSet(){
                Include = "*.csproj"
            });

            var csProjFile = files.FirstOrDefault();
            if (files.Count() > 1)
            {
                csProjFile = files.FirstOrDefault(x => x.StartsWith(project.ProjectName));
            }

            project.ProjectFile = csProjFile;

            return project;
        }

        public Project(string filename)
        {
            _directory = Path.GetDirectoryName(filename);
            _projectName = _directory.Split(Path.DirectorySeparatorChar).Last();
        }

        public string ProjectFile { get; private set; }

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

        public bool DependsOn(string nugetName)
        {
            return _nugetDependencies.Any(x => x.Name == nugetName);
        }

        public bool ShouldBeUpdated(NugetDependency latest)
        {
            if (ProjectFile.IsEmpty()) return false;

            var current = _nugetDependencies.Where(x => x.Name == latest.Name).FirstOrDefault();
            if (current != null)
            {
                return current.Version != latest.Version;
            }

            return false;
        }

        public override string ToString()
        {
            return string.Format("Project: {0}", _projectName);
        }

        public NugetDependency FindDependency(string nugetName)
        {
            return _nugetDependencies.FirstOrDefault(x => x.Name == nugetName);
        }
    }
}