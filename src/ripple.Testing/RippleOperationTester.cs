using System.Collections.Generic;
using FubuCore.CommandLine;
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
                .With(new Solution { Path = "test.config"})
                .Execute(new TestInput(), new TestCommand(new[] { s1, s2 }));

			s1.Executed.ShouldBeTrue();
			s2.Executed.ShouldBeTrue();
		}

        public class TestCommand : FubuCommand<TestInput>
        {
            private readonly IEnumerable<IRippleStep> _steps;

            public TestCommand(IEnumerable<IRippleStep> steps)
            {
                _steps = steps;
            }

            public override bool Execute(TestInput input)
            {
                return RippleOperation
                    .For<TestInput>(input)
                    .Steps(_steps)
                    .Execute(true);
            }
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