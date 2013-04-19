using NUnit.Framework;
using Rhino.Mocks;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget
{
    [TestFixture]
    public class UpdateDependencyTester
    {
        private Dependency theDependency;
        private UpdateDependency theStep;
        private INugetStepRunner theRunner;

        [SetUp]
        public void SetUp()
        {
            theDependency = new Dependency("Test");
            theStep = new UpdateDependency(theDependency);

            theRunner = MockRepository.GenerateStub<INugetStepRunner>();

            theStep.Execute(theRunner);
        }

        [Test]
        public void updates_the_dependency()
        {
            theRunner.AssertWasCalled(x => x.UpdateDependency(theDependency));
        }
    }
}