using FubuCore.CommandLine;
using ripple.Model;

namespace ripple.Commands
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
			solution.Save(true);

			new RestoreCommand().Execute(new RestoreInput
			{
				AllFlag = input.AllFlag,
				CacheFlag = input.CacheFlag,
				SolutionFlag = input.SolutionFlag
			});

			return true;
		}
	}
}