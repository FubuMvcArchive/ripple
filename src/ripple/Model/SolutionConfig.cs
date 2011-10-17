using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using FubuCore;
using System.Linq;

namespace ripple.Model
{
    public enum UpdateMode
    {
        Locked,
        Float
    }

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

        private readonly IList<string> _floats = new List<string>();

        public void FloatNuget(string nugetName)
        {
            _floats.Fill(nugetName);
        }

        public void LockNuget(string nugetName)
        {
            _floats.Remove(nugetName);
        }

        public string[] Floats
        {
            get
            {
                return _floats.ToArray();
            }
            set
            {
                _floats.Clear();
                _floats.AddRange(value);
            }
        }

        public UpdateMode ModeForNuget(string nugetName)
        {
            return _floats.Contains(nugetName) ? UpdateMode.Float : UpdateMode.Locked;
        }

        public static SolutionConfig LoadFrom(string directory)
        {
            var fileSystem = new FileSystem();
            var file = directory.AppendPath(FileName);

            return fileSystem.FileExists(file) 
                ? fileSystem.LoadFromFile<SolutionConfig>(file) 
                : null;
            
            
        }
    }
}