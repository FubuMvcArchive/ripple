using System;
using System.ComponentModel;
using FubuCore.CommandLine;
using FubuCore;
using ripple.Model;
using System.Collections.Generic;

namespace ripple.Commands
{
    public class CleanInput : SolutionInput
    {
        public CleanInput()
        {
            ModeFlag = CleanMode.all;
        }

        [Description("choose what gets cleaned up")]
        public CleanMode ModeFlag { get; set; }
    }

    [CommandDescription("Cleans out existing package folders and/or project bin & obj directories of existing build products")]
    public class CleanCommand : FubuCommand<CleanInput>
    {
        public override bool Execute(CleanInput input)
        {
            var system = new FileSystem();

            input.FindSolutions().Each(solution =>
            {
                Console.WriteLine("Cleaning Solution {0} at {1}", solution.Name, solution.Directory);

                solution.Clean(system, input.ModeFlag);
            });


            return true;
        }
    }
}