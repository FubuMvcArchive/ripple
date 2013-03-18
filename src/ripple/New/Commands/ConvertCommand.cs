using FubuCore.CommandLine;
using ripple.New.Model;

namespace ripple.New.Commands
{
	public class ConvertInput : SolutionInput
	{
	}

	public class ConvertCommand : FubuCommand<ConvertInput>
	{
		public override bool Execute(ConvertInput input)
		{
			var solution = Solution.For(input);
			solution.ConvertTo(SolutionMode.Ripple);
			solution.Save();

			return true;
		}
	}
}