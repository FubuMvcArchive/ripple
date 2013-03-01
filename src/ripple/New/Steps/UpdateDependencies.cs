using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore.Descriptions;
using ripple.New.Commands;
using ripple.New.Model;
using ripple.New.Nuget;

namespace ripple.New.Steps
{
	public class UpdateDependencies : IRippleStep, DescribesItself
	{
		public Repository Repository { get; set; }

		public void Execute(SolutionInput input, IRippleStepRunner runner)
		{
			var updates = Repository.Updates();
			var nugets = new List<INugetFile>();

			var tasks = updates.Select(x => download(x, Repository, nugets)).ToArray();
			Task.WaitAll(tasks);

			runner.Set(new DownloadedNugets(nugets));
		}

		private static Task download(IRemoteNuget nuget, Repository repository, List<INugetFile> nugets)
		{
			return Task.Factory.StartNew(() =>
			{
				RippleLog.Debug("Downloading " + nuget);
				nugets.Add(nuget.DownloadTo(repository.PackagesDirectory()));
			});
		}

		public void Describe(Description description)
		{
			var updates = Repository.Updates();

			if (updates.Any())
			{
				var list = description.AddList("Updates", Repository.Updates());
				list.Label = "Updates";
			}
			else
			{
				description.ShortDescription = "No updates found";
			}
		}
	}
}