using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;
using FubuCore;

namespace ripple
{
    [XmlType("ripple")]
    public class SolutionConfig
    {
        public static readonly string FileName = "ripple.config";

        public SolutionConfig()
        {
            NugetSpecFolder = "packaging/nuget";
            SourceFolder = "src";
            BuildCommand = "rake";
            FastBuildCommand = "rake compile";
        }

        public string Name { get; set; }
        public string NugetSpecFolder { get; set; }
        public string SourceFolder { get; set; } // look for packages.config underneath this

        public string BuildCommand { get; set; }
        public string FastBuildCommand { get; set; }
    }


    public class Solution
    {
        private readonly SolutionConfig _config;
        private readonly IList<Project> _projects = new List<Project>();

        public Solution(SolutionConfig config)
        {
            _config = config;
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
            throw new NotImplementedException();
        }

        public void AddNugetSpec(NugetSpec spec)
        {
            throw new NotImplementedException();
        }

        public void ReadDependencies(SolutionGraph graph)
        {
            throw new NotImplementedException();
        }
    }

    // Recursive -- thanks Josh
    public class NugetSpec
    {
        private readonly string _name;
        private readonly string _filename;
        private readonly IList<NugetDependency> _declarations = new List<NugetDependency>();

        public static NugetSpec ReadFrom(string filename)
        {
            throw new NotImplementedException();
        }

        public NugetSpec(string name, string filename)
        {
            _name = name;
            _filename = filename;
        }

        public string Name
        {
            get { return _name; }
        }

        public string Filename
        {
            get { return _filename; }
        }

        public IList<NugetDependency> Declarations
        {
            get { return _declarations; }
        }

        public void ReadDependencies(SolutionGraph graph)
        {
            throw new NotImplementedException();
        }

        public Solution Publisher { get; set; }

    }

    public class NugetAssembly
    {
        public NugetAssembly(string path)
        {
            // TODO -- this reads in the local path, strip out "\" in favor of "/"
        }

        public string AssemblyName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        
    }

    public class FileCopyPlan
    {
        private readonly IList<FileCopyRequest> _requests = new List<FileCopyRequest>();

        public void Copy(string from, string to)
        {
            _requests.Add(new FileCopyRequest(){
                From = from,
                To = to
            });
        }
    }

    public class FileCopyRequest
    {
        public string From { get; set; }
        public string To { get; set; }

        public bool Equals(FileCopyRequest other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.From, From) && Equals(other.To, To);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (FileCopyRequest)) return false;
            return Equals((FileCopyRequest) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((From != null ? From.GetHashCode() : 0)*397) ^ (To != null ? To.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("From: {0}, To: {1}", From, To);
        }
    }

    public class CommandLineRunner
    {
        
    }
}