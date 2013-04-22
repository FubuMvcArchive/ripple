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

			// Third time's a charm, apparently
			forceFixReferences(input);

			return true;
		}

		private void forceFixReferences(ConvertInput input)
		{
			var solution = Solution.For(input);
			solution.EachProject(x =>
			{
			    x.RemoveDuplicateReferences();

                if (x.CsProj.UsesPackagesConfig())
                {
                    x.CsProj.ConvertToRippleDependenciesConfig();
                }
			});

			solution.Save(true);
		}
	}
}