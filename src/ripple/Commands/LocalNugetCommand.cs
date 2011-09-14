using System;
using System.ComponentModel;
using FubuCore.CommandLine;
using System.Collections.Generic;
using FubuCore;

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
            var commandLine = "pack {0} -Version " + input.VersionFlag;
            commandLine += " -o " + input.DestinationFlag;

            new FileSystem().CreateDirectory(input.DestinationFlag);

            input.FindSolutions().Each(solution =>
            {
                solution.PublishedNugets.Each(spec =>
                {
                    Console.WriteLine("Building the nuget spec file at " + spec.Filename);

                    CLIRunner.RunNuget(commandLine, spec.Filename);
                    ConsoleWriter.PrintHorizontalLine();
                });
            });

            return true;
        }
    }
}