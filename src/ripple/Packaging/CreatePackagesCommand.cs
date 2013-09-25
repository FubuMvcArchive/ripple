using FubuCore.CommandLine;
using ripple.Commands;
using ripple.Steps;

namespace ripple.Packaging
{
    [CommandDescription("Creates the nuget files locally", Name = "create-packages")]
    public class CreatePackagesCommand : FubuCommand<CreatePackagesInput>
    {
        public override bool Execute(CreatePackagesInput input)
        {
            return RippleOperation
                .For(input)
                .Step<UpdateNuspecs>()
                .Step<CreatePackages>()
                .Execute();
        }
    }
}