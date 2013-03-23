using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;
using System.Linq;
using NuGet;
using ripple.Local;

namespace ripple.Commands
{
    public class LocalNugetInput : SolutionInput
    {
        public LocalNugetInput()
        {
            VersionFlag = "0.0.0.0";
            DestinationFlag = RippleFileSystem.LocalNugetDirectory();
        }

        [Description("Override the version of the nuget file")]
        public string VersionFlag { get; set; }

        [Description("Specify where the nuget file should be written, otherwise it just goes to the nuget default")]
        public string DestinationFlag { get; set; }
    }

    [CommandDescription("Creates the nuget files locally", Name = "local-nuget")]
    public class LocalNugetCommand : FubuCommand<LocalNugetInput>
    {
        public override bool Execute(LocalNugetInput input)
        {
            new FileSystem().CreateDirectory(input.DestinationFlag);

            input.EachSolution(solution =>
            {
                solution.Specifications.Each(spec => {
                    var version = spec.Dependencies.Any(x => x.Version.Contains("-"))
                                      ? input.VersionFlag + "-alpha"
                                      : input.VersionFlag;


                    RippleLog.Info("Building the nuget spec file at " + spec.Filename + " as version " + version);

					solution.Package(spec, SemanticVersion.Parse(version), input.DestinationFlag);
                    ConsoleWriter.PrintHorizontalLine();
                });
            });

            return true;
        }
    }
}