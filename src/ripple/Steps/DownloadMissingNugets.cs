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
    public class DownloadMissingNugets : IRippleStep, DescribesItself
    {
        public Solution Solution { get; set; }

        public void Execute(RippleInput input, IRippleStepRunner runner)
        {
            var feeds = Solution.Feeds.ToArray();

            if (input is IOverrideFeeds)
            {
                var overrides = input.As<IOverrideFeeds>().Feeds();
                if (overrides.Any())
                {
                    Solution.ClearFeeds();
                    Solution.AddFeeds(overrides);
                }
            }

            var missing = Solution.MissingNugets().ToList();
            var nugets = new List<INugetFile>();
            var report = new MissingNugetReport();

            if (missing.Any())
            {
                var tasks = missing.Select(x => restore(x, Solution, report, nugets)).ToArray();
                Task.WaitAll(tasks);
            }

            Solution.ClearFeeds();
            Solution.AddFeeds(feeds);

            if (!report.IsValid())
            {
                RippleLog.InfoMessage(report);
                RippleAssert.Fail("Could not restore dependencies");
            }

            runner.Set(new DownloadedNugets(nugets));
        }

        private static Task restore(Dependency query, Solution solution, MissingNugetReport report, List<INugetFile> nugets)
        {
            var result = solution.Restore(query);
            return result.ContinueWith(task =>
            {
                if (!task.Result.Found)
                {
                    report.Add(query);
                    report.AddProblems(task.Result.Problems);
                    return;
                }

                var nuget = task.Result.Nuget;
                RippleLog.Debug("Downloading " + nuget);

                nugets.Add(nuget.DownloadTo(solution, solution.PackagesDirectory()));
            }, TaskContinuationOptions.NotOnFaulted);
        }

        public void Describe(Description description)
        {
            description.ShortDescription = "Download missing nugets for " + Solution.Name;
        }
    }
}