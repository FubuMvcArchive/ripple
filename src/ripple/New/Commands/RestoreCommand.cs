using FubuCore;
using FubuCore.CommandLine;
using ripple.New.Model;
using ripple.New.Steps;

namespace ripple.New.Commands
{
	public class ReReInput : SolutionInput
	{
		public override string DescribePlan(Solution solution)
		{
			return "Restoring dependencies for solution {0} to {1}".ToFormat(solution.Name, solution.PackagesDirectory());
		}
	}

	public class ReRestoreCommand : FubuCommand<ReReInput>
	{
		public override bool Execute(ReReInput input)
		{
			return RipplePlan
				.For<ReReInput>(input)
				.Step<DownloadMissingNugets>()
				.Step<ExplodeDownloadedNugets>()
				.Execute();

			// TODO -- Need to use the INugetCache <--- Maybe proxy it through a Feed by default?
		}
	}
}