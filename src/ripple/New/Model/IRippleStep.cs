using ripple.New.Commands;

namespace ripple.New.Model
{
	public interface IRippleStep
	{
		Repository Repository { get; set; }
		void Execute(SolutionInput input, IRippleStepRunner runner);
	}
}