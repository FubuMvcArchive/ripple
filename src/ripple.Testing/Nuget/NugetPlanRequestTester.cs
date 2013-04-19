using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget
{
    [TestFixture]
    public class NugetPlanRequestTester
    {
        [Test]
        public void install_to_project()
        {
            new NugetPlanRequest { Project = "Test" }.InstallToProject().ShouldBeTrue();
        }

        [Test]
        public void install_the_project_negative()
        {
            new NugetPlanRequest().InstallToProject().ShouldBeFalse();
        }

        [Test]
        public void copy_for_dependency()
        {
            var solution = new Solution();
            var request = new NugetPlanRequest
            {
                Dependency = new Dependency("FubuCore"),
                ForceUpdates = true,
                Operation = OperationType.Update,
                Solution = solution
            };

            var copied = request.CopyFor(new Dependency("Bottles"));
            copied.Dependency.Name.ShouldEqual("Bottles");
            copied.ForceUpdates.ShouldEqual(request.ForceUpdates);
            copied.Operation.ShouldEqual(request.Operation);
            copied.Solution.ShouldEqual(request.Solution);

        }
    }
}