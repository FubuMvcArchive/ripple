using System.ComponentModel;
using FubuCore.CommandLine;
using FubuCore;
using ripple.Model;

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
            input.EachSolution(solution =>
            {
                RippleLog.Info("Cleaning Solution {0} at {1}".ToFormat(solution.Name, solution.Directory));
                solution.Clean(input.ModeFlag);
            });


            return true;
        }
    }
}