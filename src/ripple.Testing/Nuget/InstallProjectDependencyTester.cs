using NUnit.Framework;
using Rhino.Mocks;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget
{
    [TestFixture]
    public class InstallProjectDependencyTester
    {
        private Dependency theDependency;
        private string theProject;
        private InstallProjectDependency theStep;
        private INugetStepRunner theRunner;

        [SetUp]
        public void SetUp()
        {
            theDependency = new Dependency("Test");
            theProject = "Project1";
            theStep = new InstallProjectDependency(theProject, theDependency);

            theRunner = MockRepository.GenerateStub<INugetStepRunner>();

            theStep.Execute(theRunner);
        }

        [Test]
        public void installs_the_project_dependency()
        {
            theRunner.AssertWasCalled(x => x.AddProjectDependency(theProject, theDependency));
        }
    }
}