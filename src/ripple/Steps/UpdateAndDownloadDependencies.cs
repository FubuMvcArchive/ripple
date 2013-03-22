using System;
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
	public class UpdateAndDownloadDependencies : RippleStep<UpdateInput>, DescribesItself
	{
		protected override void execute(UpdateInput input, IRippleStepRunner runner)
		{
			var nugets = new List<INugetFile>();

			if (input.NugetFlag.IsNotEmpty())
			{
				updateIndividual(input, nugets);
			}
			else
			{
				updateAll(nugets);
			}

			nugets.Each(x => Solution.Update(x));

			runner.Set(new DownloadedNugets(nugets));
		}

		private void updateAll(List<INugetFile> nugets)
		{
			var updates = Solution.Updates();

			var tasks = updates.Select(x => download(x, Solution, nugets)).ToArray();
			Task.WaitAll(tasks);
		}

		private void updateIndividual(UpdateInput input, List<INugetFile> nugets)
		{
			var dependency = Solution.Dependencies.Find(input.NugetFlag);
			if (dependency.IsFixed() && !input.ForceFlag)
			{
				throw new InvalidOperationException("Cannot update 'Fixed' dependency. Please use the --force flag.");
			}

			download(Solution.UpdateFor(input.NugetFlag), Solution, nugets).Wait();
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
		}
	}
}