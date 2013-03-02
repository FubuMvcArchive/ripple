using FubuCore.Descriptions;
using ripple.New.Commands;
using ripple.New.Model;

namespace ripple.New.Steps
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