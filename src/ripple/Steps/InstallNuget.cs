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
		    var nugets = InstallationService.Install(Solution, configuration.Dependency, project, true);

			runner.Set(new DownloadedNugets(nugets));
		}

		public void Describe(Description description)
		{
			description.ShortDescription = "Install nuget and dependencies";
		}
	}
}