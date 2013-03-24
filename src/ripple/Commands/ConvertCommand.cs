using System;
using FubuCore.CommandLine;
using ripple.Model;

namespace ripple.Commands
{
	public class ConvertInput : SolutionInput
	{
		public ConvertInput()
		{
			RippleModeFlag = SolutionMode.Classic;
		}
	}

	public class ConvertCommand : FubuCommand<ConvertInput>
	{
		public override bool Execute(ConvertInput input)
		{
			var solution = Solution.For(input);
			solution.ConvertTo(SolutionMode.Ripple);
			solution.Save();

			new RestoreCommand().Execute(new RestoreInput
			{
				AllFlag = input.AllFlag,
				CacheFlag = input.CacheFlag,
				RippleModeFlag = SolutionMode.Ripple,
				SolutionFlag = input.SolutionFlag
			});

			return true;
		}
	}
}