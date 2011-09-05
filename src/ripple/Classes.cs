using System.Xml.Serialization;
using FubuCore;

namespace ripple
{
    [XmlType("ripple")]
    public class ProjectConfig
    {
        public static readonly string FileName = "ripple.config";

        public ProjectConfig()
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

    public class CommandLineRunner
    {
        
    }
}