using ripple.New.Commands;
using ripple.New.Model;

namespace ripple.New
{
	public interface IRippleStep
	{
		Solution Solution { get; set; }
		void Execute(SolutionInput input, IRippleStepRunner runner);
	}
}