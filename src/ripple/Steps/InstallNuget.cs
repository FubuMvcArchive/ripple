using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Descriptions;
using ripple.Commands;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Steps
{
	public class InstallNuget : IRippleStep, DescribesItself
	{
		public Solution Solution { get; set; }

		public void Execute(SolutionInput input, IRippleStepRunner runner)
		{
			var configuration = input.As<InstallInput>();
			if (configuration.Target == InstallationTarget.Solution)
			{
				PackageInstallation.ForSolution(configuration.Dependency).InstallTo(Solution);
				return;
			}

			var project = Solution.FindProject(configuration.ProjectFlag);
			var plan = InstallationPlan.Create(Solution, project, configuration.Dependency);

			RippleLog.DebugMessage(plan);

			plan.Installations.Each(x => PackageInstallation.ForProject(configuration.ProjectFlag, x).InstallTo(Solution));

			// TODO -- hate this
			Solution.Reset();

			var updates = Solution.Updates().Where(x => plan.Updates.Any(y => y.Name == x.Name));
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
			description.ShortDescription = "Install nuget and dependencies";
		}
	}
}