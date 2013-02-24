using System;
using System.Collections.Generic;
using System.Linq;
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
				var feeds = Repository.Feeds;
				missing.Each(query =>
				{
					IRemoteNuget nuget = null;
					foreach (var feed in feeds)
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

					RippleLog.Debug("Downloading " + nuget.ToString());
					nugets.Add(nuget.DownloadTo(Repository.PackagesDirectory()));
				});
			}

			runner.Set(new MissingNugets(nugets));
		}

		public void Describe(Description description)
		{
			description.ShortDescription = "Download missing nugets for " + Repository.Name;
		}
	}
}