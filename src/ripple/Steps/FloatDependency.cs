using FubuCore;
using ripple.Commands;

namespace ripple.Steps
{
    public class FloatDependency : RippleStep<FloatInput>
    {
        protected override void execute(FloatInput input, IRippleStepRunner runner)
        {
            var dependency = Solution.Dependencies.Find(input.Name);
            if (dependency == null)
            {
                RippleAssert.Fail("Could not find " + input.Name);
                return;
            }

            dependency.Float();

            if (input.MinVersionFlag.IsNotEmpty())
            {
                dependency.Version = input.MinVersionFlag;
            }
        }
    }
}