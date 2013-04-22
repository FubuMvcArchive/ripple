using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Commands
{
    [TestFixture]
    public class UpdateInputTester
    {
        private Solution theSolution;
        private UpdateInput theInput;

        [SetUp]
        public void SetUp()
        {
            theSolution = new Solution();
            theSolution.AddDependency(new Dependency("FubuCore"));
            theSolution.AddDependency(new Dependency("Bottles"));
            theSolution.AddDependency(new Dependency("FubuLocalization"));

            theInput = new UpdateInput();
        }

        [Test]
        public void requests_for_a_specific_nuget()
        {
            theInput.NugetFlag = "FubuCore";

            var request = theInput.Requests(theSolution).Single();
            request.Dependency.ShouldEqual(new Dependency("FubuCore"));
            request.Operation.ShouldEqual(OperationType.Update);
        }

        [Test]
        public void requests_for_a_specific_nuget_and_version()
        {
            theInput.NugetFlag = "FubuCore";
            theInput.VersionFlag = "1.1.0.0";

            var request = theInput.Requests(theSolution).Single();
            request.Dependency.ShouldEqual(new Dependency("FubuCore", "1.1.0.0"));
        }

        [Test]
        public void requests_for_a_specific_nuget_and_version_forced()
        {
            theInput.NugetFlag = "FubuCore";
            theInput.VersionFlag = "1.1.0.0";
            theInput.ForceFlag = true;

            var request = theInput.Requests(theSolution).Single();
            request.Dependency.ShouldEqual(new Dependency("FubuCore", "1.1.0.0"));
            request.ForceUpdates.ShouldBeTrue();
        }

        [Test]
        public void requests_for_entire_solution()
        {
            var requests = theInput.Requests(theSolution).ToArray();

            requests[0].Dependency.ShouldEqual(new Dependency("Bottles"));
            requests[1].Dependency.ShouldEqual(new Dependency("FubuCore"));
            requests[2].Dependency.ShouldEqual(new Dependency("FubuLocalization"));
        }
    }
}