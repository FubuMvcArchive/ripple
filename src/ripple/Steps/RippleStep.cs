using ripple.Commands;
using ripple.Model;

namespace ripple.Steps
{
	public abstract class RippleStep<T> : IRippleStep
		where T : SolutionInput
	{
		public Solution Solution { get; set; }

        public void Execute(RippleInput input, IRippleStepRunner runner)
		{
			execute((T) input, runner);
		}

		protected abstract void execute(T input, IRippleStepRunner runner);
	}
}