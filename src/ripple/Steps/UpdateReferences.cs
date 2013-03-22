using FubuCore.Descriptions;
using ripple.Commands;
using ripple.MSBuild;
using ripple.Model;

namespace ripple.Steps
{
	public class UpdateReferences : IRippleStep, DescribesItself
	{
		public Solution Solution { get; set; }

		public void Execute(SolutionInput input, IRippleStepRunner runner)
		{
			var attacher = new ReferenceAttacher(Solution);
			attacher.Attach();
		}

		public void Describe(Description description)
		{
			description.ShortDescription = "Fix References";
		}
	}
}