using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget
{
    [TestFixture]
    public class NugetPlanTester
    {
        private NugetPlan thePlan;

        [SetUp]
        public void SetUp()
        {
            thePlan = new NugetPlan();
            thePlan.AddStep(new UpdateDependency(new Dependency("d1")));
            thePlan.AddStep(new UpdateDependency(new Dependency("d2")));
        }

        private INugetStep update(string name)
        {
            return new UpdateDependency(new Dependency(name));
        }

        [Test]
        public void importing_a_plan_with_no_overlaps()
        {
            var plan2 = new NugetPlan();
            plan2.AddStep(new UpdateDependency(new Dependency("d3")));
            plan2.AddStep(new UpdateDependency(new Dependency("d4")));

            thePlan.Import(plan2);
            thePlan.ShouldHaveTheSameElementsAs(
                update("d1"),
                update("d2"),
                update("d3"),
                update("d4")
            );
        }

        [Test]
        public void importing_a_plan_with_overlaps()
        {
            var plan2 = new NugetPlan();
            plan2.AddStep(new UpdateDependency(new Dependency("d1")));
            plan2.AddStep(new UpdateDependency(new Dependency("d3")));
            plan2.AddStep(new UpdateDependency(new Dependency("d4")));

            thePlan.Import(plan2);
            thePlan.ShouldHaveTheSameElementsAs(
                update("d1"),
                update("d2"),
                update("d3"),
                update("d4")
            );
        }
    }
}