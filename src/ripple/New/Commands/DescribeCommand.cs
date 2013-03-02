using FubuCore.CommandLine;
using ripple.New.Model;

namespace ripple.New.Commands
{
	public class DescribeInput : SolutionInput
	{
	}

	public class DescribeCommand : FubuCommand<DescribeInput>
	{
		public override bool Execute(DescribeInput input)
		{
			var repository = Solution.For(input);
			RippleLog.DebugMessage(repository);

			return true;
		}
	}
}