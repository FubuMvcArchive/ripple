using System.Collections.Generic;
using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Model;
using ripple.Nuget;
using ripple.Steps;

namespace ripple.Commands
{
	public class UpdateInput : SolutionInput, INugetOperationContext
	{
	    public UpdateInput()
	    {
	        VersionFlag = "";
	    }

		[Description("Only update a specific nuget by name")]
		public string NugetFlag { get; set; }

		[Description("Only show what would be updated")]
		public bool PreviewFlag { get; set; }

        [Description("Version of the nuget")]
        [FlagAlias("version", 'v')]
        public string VersionFlag { get; set; }

		[Description("Forces the update command to override all dependencies even if they are locked")]
		[FlagAlias("force", 'f')]
		public bool ForceFlag { get; set; }

		public override string DescribePlan(Solution solution)
		{
			return "Updating dependencies for solution {0}".ToFormat(solution.Name);
		}

	    public IEnumerable<NugetPlanRequest> Requests(Solution solution)
	    {
	        if (NugetFlag.IsNotEmpty())
	        {
	            var dependency = solution.Dependencies.Find(NugetFlag);
                yield return new NugetPlanRequest
                {
                    ForceUpdates = ForceFlag,
                    Operation = OperationType.Update,
                    Dependency = new Dependency(dependency.Name, VersionFlag, dependency.Mode)
                };
	        }
	        else
	        {
                foreach (var dependency in solution.Dependencies)
                {
                    yield return requestForDependency(dependency);
                }
	        }
	    }

	    private NugetPlanRequest requestForDependency(Dependency dependency)
	    {
            return new NugetPlanRequest
            {
                ForceUpdates = ForceFlag,
                Operation = OperationType.Update,
                Dependency = new Dependency(dependency.Name, dependency.Mode)
            };
	    }
	}

	[CommandDescription("Updates the nugets for the solution")]
	public class UpdateCommand : FubuCommand<UpdateInput>
	{
		public override bool Execute(UpdateInput input)
		{
		    var solution = Solution.For(input);
            if (input.NugetFlag.IsNotEmpty() && !solution.Dependencies.Has(input.NugetFlag))
            {
                RippleAssert.Fail(input.NugetFlag + " is not a configured dependency");
                return false;
            }

            if (input.PreviewFlag)
            {
                preview(input, solution);
                return true;
            }

			return RippleOperation
				.For<UpdateInput>(input)
				.Step<NugetOperation>()
				.Step<DownloadMissingNugets>()
				.Step<ExplodeDownloadedNugets>()
				.Step<ProcessDirectives>()
				.Step<FixReferences>()
				.Execute();
		}

        private void preview(UpdateInput input, Solution solution)
        {
            var plan = NugetOperation.PlanFor(input, solution);
            RippleLog.InfoMessage(plan);
        }
	}
}