using FubuCore.Descriptions;
using ripple.Commands;
using ripple.Directives;
using ripple.MSBuild;
using ripple.Model;

namespace ripple.Steps
{
	public class FixReferences : IRippleStep, DescribesItself
	{
		public Solution Solution { get; set; }

		public void Execute(SolutionInput input, IRippleStepRunner runner)
		{
			// Opting of Mono for now. Sigh.
			if (DirectiveRunner.IsUnix())
				return;

			var attacher = new ReferenceAttacher(Solution);
			attacher.Attach();
		}

		public void Describe(Description description)
		{
			description.ShortDescription = "Fix References";
		}
	}
}