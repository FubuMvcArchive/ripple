using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using ripple.New.Model;
using ripple.New.Steps;

namespace ripple.New.Commands
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

		public Dependency Dependency
		{
			get
			{
				return new Dependency(Package, VersionFlag, ModeFlag);
			}
		}

		public PackageInstallation Installation
		{
			get { return InstallationFor(Dependency); }
		}

		public PackageInstallation InstallationFor(Dependency dependency)
		{
			if (ProjectFlag.IsNotEmpty())
			{
				return PackageInstallation.ForProject(ProjectFlag, dependency);
			}

			return PackageInstallation.ForSolution(dependency);
		}

		public override void ApplyTo(Solution solution)
		{
			Installation.InstallTo(solution);
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
			return RipplePlan
				.For<InstallInput>(input)
				.Step<InstallNuget>()
				.Execute();
		}
	}
}