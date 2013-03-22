using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Model;
using ripple.Steps;

namespace ripple.Commands
{
	public class InstallInput : SolutionInput
	{
		public InstallInput()
		{
			ModeFlag = UpdateMode.Float;
			VersionFlag = string.Empty;
		}

		[Description("The NuGet to install")]
		public string Package { get; set; }

		[Description("Version of the package")]
		[FlagAlias("Version", 'v')]
		public string VersionFlag { get; set; }

		[Description("The update mode of the package")]
		[FlagAlias("Mode", 'm')]
		public UpdateMode ModeFlag { get; set; }

		[Description("Project to install to")]
		[FlagAlias("project", 'p')]
		public string ProjectFlag { get; set; }

		public InstallationTarget Target
		{
			get { return ProjectFlag.IsNotEmpty() ? InstallationTarget.Project : InstallationTarget.Solution; }
		}

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
	}

	public class InstallCommand : FubuCommand<InstallInput>
	{
		public override bool Execute(InstallInput input)
		{
			return RippleOperation
				.For<InstallInput>(input)
				.Step<InstallNuget>()
				.Step<DownloadMissingNugets>()
				.Step<ExplodeDownloadedNugets>()
				.Step<UpdateReferences>()
				.Execute();
		}
	}
}