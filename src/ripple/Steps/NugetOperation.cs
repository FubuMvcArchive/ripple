using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using ripple.Commands;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Steps
{
    public class NugetOperation : IRippleStep
    {
        public Solution Solution { get; set; }

        public void Execute(SolutionInput input, IRippleStepRunner runner)
        {
            var nugetRunner = new NugetStepRunner(Solution);
            var aggregatePlan = PlanFor(input.As<INugetOperationContext>(), Solution);

            if (Solution.HasLockedFiles())
            {
                if (Process.GetProcessesByName("devenv.exe").Any())
                {
                    RippleAssert.Fail("Detected locked files. Do you have Visual Studio open?");
                }
                else
                {
                    RippleAssert.Fail("Detected locked files. Exiting.");
                }
                
                return;
            }

            RippleLog.InfoMessage(aggregatePlan);

            aggregatePlan.Execute(nugetRunner);
        }

        public static NugetPlan PlanFor(INugetOperationContext context, Solution solution)
        {
            var aggregatePlan = new NugetPlan();

            var requests = context.Requests(solution);
            requests.Each(request =>
            {
                request.Solution = solution;

                var plan = solution.Builder.PlanFor(request);
                aggregatePlan.Import(plan);
            });

            return aggregatePlan;
        }
    }
}