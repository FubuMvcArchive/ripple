using System.Collections.Generic;
using FubuCore;
using ripple.New.Commands;
using ripple.New.Model;
using ripple.New.Nuget;

namespace ripple.New.Steps
{
	public class InstallNuget : IRippleStep
	{
		public Solution Solution { get; set; }

		public void Execute(SolutionInput input, IRippleStepRunner runner)
		{
			var configuration = input.As<InstallInput>();

			var nugets = Solution.FeedService.DependenciesFor(Solution, configuration.Dependency);

			// TODO -- Need serious test coverage over: a) What to install and b) What to update
			nugets.Each(nuget =>
			{
				var dependency = nuget.ToDependency();
				var installation = configuration.InstallationFor(dependency);

				installation.InstallTo(Solution);
			});
		}
	}
}