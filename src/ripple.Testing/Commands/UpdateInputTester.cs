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
            request.Batched.ShouldBeFalse();
        }

        [Test]
        public void requests_for_a_specific_nuget_and_version()
        {
            theInput.NugetFlag = "FubuCore";
            theInput.VersionFlag = "1.1.0.0";

            var request = theInput.Requests(theSolution).Single();
            request.Dependency.ShouldEqual(new Dependency("FubuCore", "1.1.0.0"));
            request.Batched.ShouldBeFalse();
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
            request.Batched.ShouldBeFalse();
        }

        [Test]
        public void requests_for_entire_solution()
        {
            var requests = theInput.Requests(theSolution).ToArray();

            requests[0].Dependency.ShouldEqual(new Dependency("Bottles"));
            requests[0].Batched.ShouldBeTrue();

            requests[1].Dependency.ShouldEqual(new Dependency("FubuCore"));
            requests[1].Batched.ShouldBeTrue();

            requests[2].Dependency.ShouldEqual(new Dependency("FubuLocalization"));
            requests[2].Batched.ShouldBeTrue();
        }

        [Test]
        public void requests_for_a_single_dependency_in_a_group()
        {
            var group = new DependencyGroup();
            group.Add(new GroupedDependency("FubuCore"));
            group.Add(new GroupedDependency("FubuLocalization"));

            theSolution.AddGroup(group);
            theInput.NugetFlag = "FubuCore";

            var requests = theInput.Requests(theSolution).ToArray();

            requests[0].Dependency.ShouldEqual(new Dependency("FubuCore"));
            requests[0].Batched.ShouldBeFalse();

            requests[1].Dependency.ShouldEqual(new Dependency("FubuLocalization"));
            requests[1].Batched.ShouldBeFalse();
        }

        [Test]
        public void requests_for_a_single_dependency_in_multiple_groups()
        {
            var g1 = new DependencyGroup();
            g1.Add(new GroupedDependency("FubuCore"));
            g1.Add(new GroupedDependency("FubuLocalization"));
            theSolution.AddGroup(g1);

            var g2 = new DependencyGroup();
            g2.Add(new GroupedDependency("FubuLocalization"));
            g2.Add(new GroupedDependency("Bottles"));
            theSolution.AddGroup(g2);

            theInput.NugetFlag = "FubuCore";

            var requests = theInput.Requests(theSolution).ToArray();

            requests[0].Dependency.ShouldEqual(new Dependency("FubuCore"));
            requests[0].Batched.ShouldBeFalse();

            requests[1].Dependency.ShouldEqual(new Dependency("FubuLocalization"));
            requests[1].Batched.ShouldBeFalse();

            requests[2].Dependency.ShouldEqual(new Dependency("Bottles"));
            requests[2].Batched.ShouldBeFalse();
        }
    }
}