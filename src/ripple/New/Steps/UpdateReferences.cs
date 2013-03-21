using FubuCore.Descriptions;
using ripple.MSBuild;
using ripple.Model;
using ripple.New.Commands;
using ripple.New.Model;

namespace ripple.New.Steps
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