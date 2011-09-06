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

        public static SolutionConfig LoadFrom(string directory)
        {
            return new FileSystem().LoadFromFile<SolutionConfig>(directory.AppendPath(FileName));
        }
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