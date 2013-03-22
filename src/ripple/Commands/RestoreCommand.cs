using FubuCore;
using FubuCore.CommandLine;
using ripple.Model;
using ripple.Steps;

namespace ripple.Commands
{
	public class RestoreInput : SolutionInput
	{
		public override string DescribePlan(Solution solution)
		{
			return "Restoring dependencies for solution {0} to {1}".ToFormat(solution.Name, solution.PackagesDirectory());
		}
	}

	public class RestoreCommand : FubuCommand<RestoreInput>
	{
		public override bool Execute(RestoreInput input)
		{
			return RippleOperation
				.For<RestoreInput>(input)
				.Step<DownloadMissingNugets>()
				.Step<ExplodeDownloadedNugets>()
				.Step<UpdateReferences>()
				.Execute();
		}
	}
}