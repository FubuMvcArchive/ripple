using FubuCore.CommandLine;
using ripple.Model;

namespace ripple.Commands
{
	public class DescribeInput : SolutionInput
	{
	}

	public class DescribeCommand : FubuCommand<DescribeInput>
	{
		public override bool Execute(DescribeInput input)
		{
			RippleLog.Verbose(true);

			var solution = Solution.For(input);
			RippleLog.DebugMessage(solution);

			return true;
		}
	}
}