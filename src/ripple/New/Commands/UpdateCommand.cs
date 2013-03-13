using FubuCore;
using FubuCore.CommandLine;
using ripple.New.Model;
using ripple.New.Steps;

namespace ripple.New.Commands
{
	public class ReUpdateInput : SolutionInput
	{
		public override string DescribePlan(Solution solution)
		{
			return "Updating dependencies for solution {0}".ToFormat(solution.Name);
		}
	}

	public class ReUpdateCommand : FubuCommand<ReUpdateInput>
	{
		public override bool Execute(ReUpdateInput input)
		{
			return RipplePlan
				.For<ReUpdateInput>(input)
				.Step<DownloadUpdatedDependencies>()
				.Step<ExplodeDownloadedNugets>()
				.Step<UpdateReferences>()
				.Execute();
		}
	}
}