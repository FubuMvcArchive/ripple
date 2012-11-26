using System;
using System.Collections.Generic;
using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using System.Linq;

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
            var commandLine = "pack {0} -Version {1}";
            commandLine += " -o " + input.DestinationFlag.FileEscape();

            new FileSystem().CreateDirectory(input.DestinationFlag);

            input.FindSolutions().Each(solution =>
            {
                solution.PublishedNugets.Each(spec => {
                    var version = spec.Dependencies.Any(x => x.Version.Contains("-"))
                                      ? input.VersionFlag + "-alpha"
                                      : input.VersionFlag;


                    Console.WriteLine("Building the nuget spec file at " + spec.Filename + " as version " + version);

                    CLIRunner.RunNuget(commandLine, spec.Filename.FileEscape(), version);
                    ConsoleWriter.PrintHorizontalLine();
                });
            });

            return true;
        }
    }
}