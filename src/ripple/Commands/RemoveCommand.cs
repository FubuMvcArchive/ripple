using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Model;
using ripple.Steps;

namespace ripple.Commands
{
    public class RemoveInput : SolutionInput
    {
        [Description("The name of the nuget to completely remove from the solution")]
        public string Nuget { get; set; }

        [Description("Project to remove from")]
        [FlagAlias("project", 'p')]
        public string ProjectFlag { get; set; }

        public override void ApplyTo(Solution solution)
        {
            solution.RequestSave();
        }
    }

	[CommandDescription("Removes a nuget from the solution")]
    public class RemoveCommand : FubuCommand<RemoveInput>
    {
        public override bool Execute(RemoveInput input)
        {
            RippleLog.Info("Trying to remove {0}".ToFormat(input.Nuget));

            return RippleOperation
                .For(input)
                .Step<RemoveNuget>()
                .ForceSave()
                .Execute();
        }
    }
}