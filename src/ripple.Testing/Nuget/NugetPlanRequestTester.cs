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

        [Test]
        public void is_transitive()
        {
            var request = new NugetPlanRequest
            {
                Dependency = new Dependency("FubuCore"),
                ForceUpdates = true,
                Operation = OperationType.Update,
            };

            request.IsTransitive().ShouldBeFalse();
        }

        [Test]
        public void copying_the_request_marks_as_transitive()
        {
            var request = new NugetPlanRequest
            {
                Dependency = new Dependency("FubuCore"),
                ForceUpdates = true,
                Operation = OperationType.Update,
            };

            request.CopyFor(new Dependency("Bottles")).IsTransitive().ShouldBeTrue();
        }

        [Test]
        public void copying_the_request_sets_the_parent()
        {
            var request = new NugetPlanRequest
            {
                Dependency = new Dependency("FubuCore"),
                ForceUpdates = true,
                Operation = OperationType.Update,
            };

            request.CopyFor(new Dependency("Bottles")).Parent.ShouldEqual(request);
        }

        [Test]
        public void should_update_forced_transitive_install()
        {
            var request = new NugetPlanRequest
            {
                Dependency = new Dependency("FubuCore"),
                ForceUpdates = true,
                Operation = OperationType.Install,
            };

            request.CopyFor(new Dependency("Bottles", "1.0.0.0", UpdateMode.Fixed)).ShouldUpdate(new Dependency("Bottles")).ShouldBeTrue();
        }

        [Test]
        public void should_update_forced_update()
        {
            var request = new NugetPlanRequest
            {
                Dependency = new Dependency("FubuCore"),
                ForceUpdates = true,
                Operation = OperationType.Update,
            };

            request.ShouldUpdate(new Dependency("FubuCore", "1.0.0.0", UpdateMode.Fixed)).ShouldBeTrue();
        }

        [Test]
        public void should_update_transitive_floats()
        {
            var request = new NugetPlanRequest
            {
                Dependency = new Dependency("FubuCore"),
                ForceUpdates = false,
                Operation = OperationType.Install,
            };

            request.CopyFor(new Dependency("Bottles", "1.0.0.0", UpdateMode.Fixed)).ShouldUpdate(new Dependency("FubuCore", "1.0.0.0", UpdateMode.Float)).ShouldBeTrue();
        }

        [Test]
        public void should_update_floats_when_forced()
        {
            var request = new NugetPlanRequest
            {
                Dependency = new Dependency("FubuCore"),
                ForceUpdates = true,
                Operation = OperationType.Update,
            };

            request.ShouldUpdate(new Dependency("FubuCore", "1.0.0.0", UpdateMode.Float)).ShouldBeTrue();
        }

        [Test]
        public void update_for_fixed_when_not_forced_and_not_batched()
        {
            var request = new NugetPlanRequest
            {
                Dependency = new Dependency("FubuCore"),
                ForceUpdates = false,
                Operation = OperationType.Update,
            };

            request.ShouldUpdate(new Dependency("FubuCore", "1.0.0.0", UpdateMode.Fixed)).ShouldBeTrue();
        }

        [Test]
        public void no_update_for_fixed_when_not_forced_and_batched()
        {
            var request = new NugetPlanRequest
            {
                Dependency = new Dependency("FubuCore"),
                ForceUpdates = false,
                Batched = true,
                Operation = OperationType.Update,
            };

            request.ShouldUpdate(new Dependency("FubuCore", "1.0.0.0", UpdateMode.Fixed)).ShouldBeFalse();
        }

        [Test]
        public void no_update_for_float_when_not_an_update()
        {
            var request = new NugetPlanRequest
            {
                Dependency = new Dependency("FubuCore"),
                ForceUpdates = false,
                Operation = OperationType.Install,
            };

            request.ShouldUpdate(new Dependency("FubuCore", "1.0.0.0", UpdateMode.Float)).ShouldBeFalse();
        }

        [Test]
        public void original_dependency()
        {
            var request = new NugetPlanRequest
            {
                Dependency = new Dependency("FubuCore"),
                ForceUpdates = false,
                Operation = OperationType.Install,
            };

            request.Origin().ShouldEqual(request.Dependency);
        }

        [Test]
        public void original_dependency_recursive()
        {
            var request = new NugetPlanRequest
            {
                Dependency = new Dependency("FubuCore"),
                ForceUpdates = false,
                Operation = OperationType.Install,
            };

            request.CopyFor(new Dependency("Bottles"))
                   .CopyFor(new Dependency("Something"))
                   .Origin()
                   .ShouldEqual(request.Dependency);
        }

        [Test]
        public void should_force_update_for_the_original_dependency_when_not_batched()
        {
            var request = new NugetPlanRequest
            {
                Dependency = new Dependency("FubuCore"),
                ForceUpdates = false,
                Batched = false,
                Operation = OperationType.Install,
            };

            request.CopyFor(new Dependency("Bottles"))
                   .CopyFor(new Dependency("Something"))
                   .ShouldForce(request.Dependency)
                   .ShouldBeTrue();
        }

        [Test]
        public void should_not_force_update_for_the_original_dependency_when_batched()
        {
            var request = new NugetPlanRequest
            {
                Dependency = new Dependency("FubuCore"),
                ForceUpdates = false,
                Batched = true,
                Operation = OperationType.Install,
            };

            request.CopyFor(new Dependency("Bottles"))
                   .CopyFor(new Dependency("Something"))
                   .ShouldForce(request.Dependency)
                   .ShouldBeFalse();
        }
    }
}