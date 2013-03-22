using System;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;
using ripple.Local;
using ripple.Model;

namespace ripple.Testing
{
    [TestFixture]
    public class when_executing_successfully
    {
        private Solution theSolution;
        private BuildSolution theBuild;
        private Local.IRippleStepRunner theRunner;

        [SetUp]
        public void SetUp()
        {
            theSolution = new Solution();
            theBuild = new BuildSolution(theSolution);

            theRunner = MockRepository.GenerateMock<Local.IRippleStepRunner>();

            theBuild.Execute(theRunner);
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
        private Local.IRippleStepRunner theRunner;
        private NotImplementedException theException;

        [SetUp]
        public void SetUp()
        {
            theSolution = new Solution();
            theBuild = new BuildSolution(theSolution);

            theRunner = MockRepository.GenerateMock<Local.IRippleStepRunner>();
            theException = new NotImplementedException();

            theRunner.Expect(x => x.BuildSolution(theSolution)).Throw(theException);

            Exception<NotImplementedException>.ShouldBeThrownBy(() =>
            {
                theBuild.Execute(theRunner);
            }).ShouldBeTheSameAs(theException);

            
        }

        [Test]
        public void should_trace_how_to_open_the_solution()
        {
            theRunner.AssertWasCalled(x => x.Trace(BuildSolution.OpenLogMessage, theSolution.Name));
        }
    }
}