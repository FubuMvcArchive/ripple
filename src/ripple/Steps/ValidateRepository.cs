using FubuCore.Descriptions;
using ripple.Commands;
using ripple.Model;

namespace ripple.Steps
{
	public class ValidateRepository : IRippleStep, DescribesItself
	{
		public Solution Solution { get; set; }

		public void Execute(SolutionInput input, IRippleStepRunner runner)
		{
			Solution.AssertIsValid();
		}

		public void Describe(Description description)
		{
			description.ShortDescription = "Validate " + Solution.Name;
		}
	}
}