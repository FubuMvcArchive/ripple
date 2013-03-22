using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;

namespace ripple.Testing
{
	[TestFixture]
	public class RippleOperationTester
	{
		[Test]
		public void executes_the_steps()
		{
			var s1 = new RecordingStep();
			var s2 = new RecordingStep();

			RippleOperation
				.For<TestInput>(new TestInput(), new Solution { Path = "test.config"})
				.Step(s1)
				.Step(s2)
				.Execute();

			s1.Executed.ShouldBeTrue();
			s2.Executed.ShouldBeTrue();
		}
	}

	public class TestInput : SolutionInput { }

	public class RecordingStep : IRippleStep
	{
		public Solution Solution { get; set; }
		public bool Executed { get; set; }

		public void Execute(SolutionInput input, IRippleStepRunner runner)
		{
			Executed = true;
		}
	}
}