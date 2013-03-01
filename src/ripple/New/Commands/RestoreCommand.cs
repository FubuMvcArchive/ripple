using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;
using ripple.New.Model;
using ripple.New.Nuget;
using ripple.New.Steps;

namespace ripple.New.Commands
{
	public class SolutionInput
	{
		[Description("override the solution to be cleaned")]
		[FlagAlias("solution", 'l')]
		public string SolutionFlag { get; set; }
	}

	public class ReReInput : SolutionInput
	{
	}

	public class ReRestoreCommand : FubuCommand<ReReInput>
	{
		public override bool Execute(ReReInput input)
		{
			var repository = Repository.For(input);
			var packagesDir = repository.PackagesDirectory();

			RippleLog.Info("Restoring dependencies for solution {0} to {1}".ToFormat(repository.Name, packagesDir));

			var plan = RipplePlan
				.For<ReReInput>(input, repository)
				.Step<DownloadMissingNugets>()
				.Step<ExplodeDownloadedNugets>();

			RippleLog.DebugMessage(plan);

			return plan.Execute();

			// TODO -- Need to use the INugetCache <--- Maybe proxy it through a Feed by default?
		}
	}
}