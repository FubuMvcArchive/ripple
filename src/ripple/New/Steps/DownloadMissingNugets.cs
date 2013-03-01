using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Descriptions;
using ripple.New.Commands;
using ripple.New.Model;
using ripple.New.Nuget;

namespace ripple.New.Steps
{
	public class DownloadMissingNugets : IRippleStep, DescribesItself
	{
		public Repository Repository { get; set; }

		public void Execute(SolutionInput input, IRippleStepRunner runner)
		{
			var missing = Repository.MissingNugets().ToList();
			var nugets = new List<INugetFile>();

			if (missing.Any())
			{
				var tasks = missing.Select(x => restore(x, Repository, nugets)).ToArray();

				Task.WaitAll(tasks);
			}

			runner.Set(new DownloadedNugets(nugets));
		}

		private static Task restore(NugetQuery query, Repository repository, List<INugetFile> nugets)
		{
			return Task.Factory.StartNew(() =>
			{
				IRemoteNuget nuget = null;
				foreach (var feed in repository.Feeds)
				{
					try
					{
						nuget = feed.Find(query);
						break;
					}
					catch (ArgumentOutOfRangeException)
					{
					}
				}

				if (nuget == null)
				{
					RippleLog.Error("Could not find {0}".ToFormat(query), new ArgumentOutOfRangeException());
					return;
				}

				RippleLog.Debug("Downloading " + nuget);
				nugets.Add(nuget.DownloadTo(repository.PackagesDirectory()));
			});
		}

		public void Describe(Description description)
		{
			description.ShortDescription = "Download missing nugets for " + Repository.Name;
		}
	}
}