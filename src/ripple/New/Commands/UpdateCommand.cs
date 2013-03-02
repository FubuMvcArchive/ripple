using FubuCore;
using FubuCore.CommandLine;
using ripple.New.Model;
using ripple.New.Steps;

namespace ripple.New.Commands
{
	public class ReUpdateInput : SolutionInput
	{
	}

	public class ReUpdateCommand : FubuCommand<ReUpdateInput>
	{
		public override bool Execute(ReUpdateInput input)
		{
			var repository = Solution.For(input);

			RippleLog.Info("Updating dependencies for solution {0}".ToFormat(repository.Name));

			var plan = RipplePlan
				.For<ReUpdateInput>(input, repository)
				.Step<UpdateDependencies>()
				.Step<ExplodeDownloadedNugets>();

			RippleLog.DebugMessage(plan);

			return plan.Execute();
		}
	}
}