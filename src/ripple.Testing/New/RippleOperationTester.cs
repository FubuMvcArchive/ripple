using FubuTestingSupport;
using NUnit.Framework;
using ripple.New;
using ripple.New.Commands;
using ripple.New.Model;

namespace ripple.Testing.New
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