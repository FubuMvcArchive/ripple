using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore.Descriptions;
using ripple.Commands;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Steps
{
	public class DownloadUpdatedDependencies : IRippleStep, DescribesItself
	{
		public Solution Solution { get; set; }

		public void Execute(SolutionInput input, IRippleStepRunner runner)
		{
			var updates = Solution.Updates();
			var nugets = new List<INugetFile>();

			var tasks = updates.Select(x => download(x, Solution, nugets)).ToArray();
			Task.WaitAll(tasks);

			runner.Set(new DownloadedNugets(nugets));
		}

		private static Task download(IRemoteNuget nuget, Solution solution, List<INugetFile> nugets)
		{
			return Task.Factory.StartNew(() =>
			{
				RippleLog.Debug("Downloading " + nuget);
				nugets.Add(nuget.DownloadTo(solution, solution.PackagesDirectory()));
			});
		}

		public void Describe(Description description)
		{
			var updates = Solution.Updates();

			if (updates.Any())
			{
				var list = description.AddList("Updates", Solution.Updates());
				list.Label = "Updates";
			}
			else
			{
				description.ShortDescription = "No updates found";
			}
		}
	}
}