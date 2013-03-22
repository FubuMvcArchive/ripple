using ripple.Commands;
using ripple.Model;

namespace ripple
{
	public interface IRippleStep
	{
		Solution Solution { get; set; }
		void Execute(SolutionInput input, IRippleStepRunner runner);
	}
}