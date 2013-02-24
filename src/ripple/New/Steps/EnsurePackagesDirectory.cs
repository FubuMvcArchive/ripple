using FubuCore;
using FubuCore.Descriptions;
using ripple.New.Commands;
using ripple.New.Model;

namespace ripple.New.Steps
{
	public class EnsurePackagesDirectory : IRippleStep, DescribesItself
	{
		public Repository Repository { get; set; }

		public void Execute(SolutionInput input, IRippleStepRunner runner)
		{
			runner.CreateDirectory(Repository.PackagesDirectory());
		}

		public void Describe(Description description)
		{
			description.ShortDescription = "Ensure that the {0} directory exists".ToFormat(Repository.PackagesDirectory());
		}
	}
}