using System.Collections.Generic;
using System.Linq;
using FubuCore.CommandLine;
using ripple.Model;
using ripple.Nuget;
using ripple.Steps;

namespace ripple.Commands
{
    public class FixInput : RippleInput, INugetOperationContext
    {
        public override void ApplyTo(Solution solution)
        {
            solution.RequestSave();
        }

        public IEnumerable<NugetPlanRequest> Requests(Solution solution)
        {
            return solution
                .Dependencies
                .Select(dependency => new NugetPlanRequest
                {
                    ForceUpdates = false,
                    Operation = OperationType.Install,
                    Batched = true,
                    Dependency = dependency
                });
        }
    }

    [CommandDescription("Analyzes solution and project dependencies and fixes missing dependencies and references")]
    public class FixCommand : FubuCommand<FixInput>
    {
        public override bool Execute(FixInput input)
        {
            return RippleOperation
                .For(input)
                .Step<NugetOperation>()
                .Step<DownloadMissingNugets>()
                .Step<ExplodeDownloadedNugets>()
                .Step<FixReferences>()
                .Execute();
        }
    }
}