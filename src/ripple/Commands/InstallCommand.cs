using System.Collections.Generic;
using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Model;
using ripple.Nuget;
using ripple.Steps;

namespace ripple.Commands
{
    public class InstallInput : RippleInput, INugetOperationContext
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

        [Description("Only show what would be installed")]
        public bool PreviewFlag { get; set; }

        [Description("Override the feed-level stability configuration")]
        [FlagAlias("stability", 's')]
        public NugetStability? StabilityFlag { get; set; }

        public Dependency Dependency
        {
            get
            {
                return new Dependency(Package, VersionFlag, ModeFlag) { NugetStability = StabilityFlag };
            }
        }

        public override void ApplyTo(Solution solution)
        {
            solution.RequestSave();
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
            var solution = Solution.For(input);

            if (input.ProjectFlag.IsNotEmpty())
            {
                var project = solution.FindProject(input.ProjectFlag);
                if (project == null)
                {
                    RippleAssert.Fail("Project " + input.ProjectFlag + " does not exist");
                    return false;
                }

                if (project.Dependencies.Has(input.Package))
                {
                    RippleAssert.Fail(input.Package + " already exists in Project " + input.ProjectFlag);
                    return false;
                }
            }
            else if (solution.Dependencies.Has(input.Package))
            {
                RippleAssert.Fail(input.Package + " already exists in solution");
                return false;
            }

            if (input.PreviewFlag)
            {
                preview(input, solution);
                return true;
            }

            return RippleOperation
                .For(input)
                .Step<NugetOperation>()
                .Step<DownloadMissingNugets>()
                .Step<ExplodeDownloadedNugets>()
                .Step<FixReferences>()
                .Execute();
        }

        private void preview(InstallInput input, Solution solution)
        {
            var plan = NugetOperation.PlanFor(input, solution);
            RippleLog.InfoMessage(plan);
        }
    }
}