using ripple.Steps;

namespace ripple.Packaging
{
    public class GenerateNuspecs : RippleStep<CreatePackagesInput>
    {
        protected override void execute(CreatePackagesInput input, IRippleStepRunner runner)
        {
            // TODO -- Get this from the solution
            var generator = NuspecGenerator.Basic();
            var plan = generator.PlanFor(Solution, input.Version());

            if (input.PreviewFlag)
            {
                RippleLog.InfoMessage(plan);
                return;
            }

            RippleLog.DebugMessage(plan);

            var report = plan.Execute(input.UpdateDependenciesFlag);
            runner.Set(report);
        }
    }
}