using System.ComponentModel;
using FubuCore.CommandLine;
using NuGet;
using ripple.Commands;

namespace ripple.Packaging
{
    public class CreatePackagesInput : SolutionInput
    {
        public CreatePackagesInput()
        {
            VersionFlag = "0.0.0.0";
            DestinationFlag = RippleFileSystem.LocalNugetDirectory();
        }

        [Description("Override the version of the nuget file")]
        public string VersionFlag { get; set; }

        [Description("Specify where the nuget file should be written, otherwise it just goes to the ripple default")]
        public string DestinationFlag { get; set; }

        [Description("Only show what would be updated/created")]
        [FlagAlias("preview", 'p')]
        public bool PreviewFlag { get; set; }

        [Description("Update the nuspec files to match the current dependency version constraints")]
        [FlagAlias("update-dependencies", 'u')]
        public bool UpdateDependenciesFlag { get; set; }

        [Description("Also create symbols packages")]
        [FlagAlias("symbols", 'C')]
        public bool CreateSymbolsFlag { get; set; }

        public SemanticVersion Version()
        {
            return new SemanticVersion(VersionFlag);
        }
    }
}