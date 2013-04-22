using System.Collections.Generic;
using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Model;
using ripple.Nuget;
using ripple.Steps;

namespace ripple.Commands
{
	public class InstallInput : SolutionInput, INugetOperationContext
	{
		public InstallInput()
		{
			ModeFlag = UpdateMode.Float;
			VersionFlag = string.Empty;
		}

		[Description("The NuGet to install")]
		public string Package { get; set; }

		[Description("Version of the package")]
		[FlagAlias("version", 'v')]
		public string VersionFlag { get; set; }

		[Description("The update mode of the package")]
		[FlagAlias("ModeFlag", 'm')]
		public UpdateMode ModeFlag { get; set; }

		[Description("Project to install to")]
		[FlagAlias("project", 'p')]
		public string ProjectFlag { get; set; }

        [Description("Forces updates of transitive updates")]
        [FlagAlias("force-updates", 'f')]
        public bool ForceUpdatesFlag { get; set; }

		public Dependency Dependency
		{
			get
			{
				return new Dependency(Package, VersionFlag, ModeFlag);
			}
		}

		public override string DescribePlan(Solution solution)
		{
			return "Install {0} to Solution {1}".ToFormat(Package, solution.Name);
		}

        public IEnumerable<NugetPlanRequest> Requests(Solution solution)
	    {
	        yield return new NugetPlanRequest
	        {
                Dependency = Dependency,
                ForceUpdates = ForceUpdatesFlag,
                Operation = OperationType.Install,
                Project = ProjectFlag
	        };
	    }
	}

	public class InstallCommand : FubuCommand<InstallInput>
	{
		public override bool Execute(InstallInput input)
		{
			return RippleOperation
				.For<InstallInput>(input)
				.Step<NugetOperation>()
				.Step<DownloadMissingNugets>()
				.Step<ExplodeDownloadedNugets>()
				.Step<FixReferences>()
				.Execute();
		}
	}
}