using System;
using FubuCore.Descriptions;
using ripple.Commands;

namespace ripple.Steps
{
	public class UpdateAndDownloadDependencies : RippleStep<UpdateInput>, DescribesItself
	{
		protected override void execute(UpdateInput input, IRippleStepRunner runner)
		{
			throw new NotImplementedException();
		}

		public void Describe(Description description)
		{
		}
	}
}