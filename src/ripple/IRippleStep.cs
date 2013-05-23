using ripple.Commands;
using ripple.Model;

namespace ripple
{
	public interface IRippleStep
	{
		Solution Solution { get; set; }
        void Execute(RippleInput input, IRippleStepRunner runner);
	}
}