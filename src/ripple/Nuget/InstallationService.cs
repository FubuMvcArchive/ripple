using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ripple.Model;

namespace ripple.Nuget
{
    public class InstallationService
    {
        public static IEnumerable<INugetFile> Install(Solution solution, Dependency dependency, Project project, bool force)
        {
            var plan = InstallationPlan.Create(solution, project, dependency, force);

            RippleLog.DebugMessage(plan);

            plan.Installations.Each(x => PackageInstallation.ForProject(project, x).InstallTo(solution));

            // TODO -- hate this
            solution.Reset();

            var nugets = new List<INugetFile>();

            var tasks = plan.Updates.Select(x =>
            {
                var nuget = solution.FeedService.UpdateFor(solution, x, force);
                return download(nuget, solution, nugets);
            }).ToArray();

            Task.WaitAll(tasks);

            nugets.Each(nuget => solution.Update(nuget));

            return nugets;
        }

        private static Task download(IRemoteNuget nuget, Solution solution, List<INugetFile> nugets)
        {
            return Task.Factory.StartNew(() =>
            {
                RippleLog.Debug("Downloading " + nuget);
                nugets.Add(nuget.DownloadTo(solution, solution.PackagesDirectory()));
            });
        }
    }
}