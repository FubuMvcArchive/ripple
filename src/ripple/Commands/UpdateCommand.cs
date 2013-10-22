using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Model;
using ripple.Nuget;
using ripple.Steps;

namespace ripple.Commands
{
    public class UpdateInput : RippleInput, INugetOperationContext
    {
        public UpdateInput()
        {
            VersionFlag = "";
        }

        [Description("Only update a specific nuget by name")]
        public string NugetFlag { get; set; }

        [Description("Only show what would be updated")]
        public bool PreviewFlag { get; set; }

        [Description("Version of the nuget")]
        [FlagAlias("version", 'V')]
        public string VersionFlag { get; set; }

        [Description("Forces the update command to override all dependencies even if they are locked")]
        [FlagAlias("force", 'f')]
        public bool ForceFlag { get; set; }

        [Description("Override the feed-level stability configuration")]
        [FlagAlias("stability", 's')]
        public NugetStability? StabilityFlag { get; set; }

        public override string DescribePlan(Solution solution)
        {
            return "Updating dependencies for solution {0}".ToFormat(solution.Name);
        }

        public override void ApplyTo(Solution solution)
        {
            solution.RequestSave();
        }

        public IEnumerable<NugetPlanRequest> Requests(Solution solution)
        {
            if (NugetFlag.IsNotEmpty())
            {
                var requests = new List<NugetPlanRequest>();
                gatherDependencies(requests, solution, NugetFlag);

                return requests;
            }

            return solution.Dependencies.Select(requestForDependency);
        }

        private NugetPlanRequest requestForExisting(Solution solution, string name)
        {
            var dependency = solution.Dependencies.Find(name);
            return new NugetPlanRequest
            {
                ForceUpdates = ForceFlag,
                Operation = OperationType.Update,
                Dependency = new Dependency(dependency.Name, VersionFlag, dependency.Mode) { NugetStability = StabilityFlag }
            };
        }

        private void gatherDependencies(List<NugetPlanRequest> requests, Solution solution, string name)
        {
            if (!solution.Groups.Any(group => group.Has(name)))
            {
                requests.Fill(requestForExisting(solution, name));
                return;
            }


            var dependencies = solution
                .Groups
                .Where(group => group.Has(name))
                .SelectMany(group => group.GroupedDependencies.Select(x => x.Name))
                .Distinct();

            var newItems = new List<string>();
            dependencies.Each(x =>
            {
                if (!requests.Any(r => r.Dependency.MatchesName(x)))
                {
                    newItems.Add(x);
                    requests.Fill(requestForExisting(solution, x));
                }
            });

            newItems.Each(x => gatherDependencies(requests, solution, x));
        }

        private NugetPlanRequest requestForDependency(Dependency dependency)
        {
            return new NugetPlanRequest
            {
                ForceUpdates = ForceFlag,
                Operation = OperationType.Update,
                Batched = true,
                Dependency = new Dependency(dependency.Name, dependency.Mode)
            };
        }
    }

    [CommandDescription("Updates the nugets for the solution")]
    public class UpdateCommand : FubuCommand<UpdateInput>
    {
        public override bool Execute(UpdateInput input)
        {
            var solution = Solution.For(input);
            if (input.NugetFlag.IsNotEmpty() && !solution.Dependencies.Has(input.NugetFlag))
            {
                RippleAssert.Fail(input.NugetFlag + " is not a configured dependency");
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
                .Step<ProcessDirectives>()
                .Step<FixReferences>()
                .Execute();
        }

        private void preview(UpdateInput input, Solution solution)
        {
            var plan = NugetOperation.PlanFor(input, solution);
            RippleLog.InfoMessage(plan);
        }
    }
}