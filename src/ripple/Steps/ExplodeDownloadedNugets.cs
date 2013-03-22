using System.Collections.Generic;
using FubuCore.Descriptions;
using ripple.Commands;
using ripple.Model;

namespace ripple.Steps
{
	public class ExplodeDownloadedNugets : IRippleStep, DescribesItself
	{
		public Solution Solution { get; set; }

		public void Execute(SolutionInput input, IRippleStepRunner runner)
		{
			var missing = runner.Get<DownloadedNugets>();
			missing.Each(nuget =>
			{
				var dir = Solution.PackagesDirectory();
				RippleLog.Debug("Exploding " + nuget.ToString());
				nuget.ExplodeTo(dir);
			});
		}

		public void Describe(Description description)
		{
			description.ShortDescription = "Explode nugets at " + Solution.PackagesDirectory();
		}
	}
}