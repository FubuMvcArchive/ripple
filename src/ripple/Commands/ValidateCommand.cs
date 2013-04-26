using System.ComponentModel;
using FubuCore.CommandLine;

namespace ripple.Commands
{
    [Description("Validates the current solution and dependencies")]
    public class ValidateCommand : FubuCommand<SolutionInput>
    {
        public override bool Execute(SolutionInput input)
        {
            input.EachSolution(solution => solution.AssertIsValid());

            return true;
        }
    }
}