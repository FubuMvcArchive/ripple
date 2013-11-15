using FubuCore.CommandLine;

namespace ripple.Packaging
{
    [CommandDescription("Creates the nuget files locally", Name = "create-packages")]
    public class CreatePackagesCommand : FubuCommand<CreatePackagesInput>
    {
        public override bool Execute(CreatePackagesInput input)
        {
            var operation = RippleOperation.For(input);

            // TODO -- Opt out of the package templating
            operation.Step<GenerateNuspecs>();

            if (!input.PreviewFlag)
            {
                operation.Step<CreatePackages>();
            }

            return operation.Execute();
        }
    }
}