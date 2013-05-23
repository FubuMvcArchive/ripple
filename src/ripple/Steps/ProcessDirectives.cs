using FubuCore.Descriptions;
using ripple.Commands;
using ripple.Directives;
using ripple.Model;

namespace ripple.Steps
{
	public class ProcessDirectives : IRippleStep, DescribesItself
	{
		public Solution Solution { get; set; }

        public void Execute(RippleInput input, IRippleStepRunner runner)
		{
			DirectiveProcessor.ProcessDirectives(Solution);
		}

		public void Describe(Description description)
		{
			description.ShortDescription = "Process directives from nugets";
		}
	}
}