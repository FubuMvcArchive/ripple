using System;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace ripple.Testing
{
    [TestFixture]
    public class when_executing_successfully
    {
        private Solution theSolution;
        private BuildSolution theBuild;
        private IRippleRunner theRunner;
        private RippleStepResult theResult;

        [SetUp]
        public void SetUp()
        {
            theSolution = new Solution(new SolutionConfig(), "some directory");
            theBuild = new BuildSolution(theSolution);

            theRunner = MockRepository.GenerateMock<IRippleRunner>();

            theResult = theBuild.Execute(theRunner);
        }

        [Test]
        public void the_result_should_be_successful()
        {
            theResult.Success.ShouldBeTrue();
        }

        [Test]
        public void should_build_the_actual_solution_with_the_runner()
        {
            theRunner.AssertWasCalled(x => x.BuildSolution(theSolution));
        }

        [Test]
        public void should_trace_how_to_open_the_solution()
        {
            theRunner.AssertWasCalled(x => x.Trace(BuildSolution.OpenLogMessage, theSolution.Name));
        }
    }

    [TestFixture]
    public class when_executing_and_the_build_fails
    {
        private Solution theSolution;
        private BuildSolution theBuild;
        private IRippleRunner theRunner;
        private RippleStepResult theResult;
        private NotImplementedException theException;

        [SetUp]
        public void SetUp()
        {
            theSolution = new Solution(new SolutionConfig(), "some directory");
            theBuild = new BuildSolution(theSolution);

            theRunner = MockRepository.GenerateMock<IRippleRunner>();
            theException = new NotImplementedException();

            theRunner.Expect(x => x.BuildSolution(theSolution)).Throw(theException);

            theResult = theBuild.Execute(theRunner);
        }

        [Test]
        public void the_result_should_be_a_failure()
        {
            theResult.Success.ShouldBeFalse();
        }

        [Test]
        public void the_result_message_should_be_from_the_exception()
        {
            theResult.Message.ShouldEqual(theException.ToString());
        }

        [Test]
        public void should_trace_how_to_open_the_solution()
        {
            theRunner.AssertWasCalled(x => x.Trace(BuildSolution.OpenLogMessage, theSolution.Name));
        }
    }
}