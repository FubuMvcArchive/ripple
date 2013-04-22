using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using ripple.Commands;
using ripple.Model;
using ripple.Nuget;
using ripple.Steps;

namespace ripple.Testing.Steps
{
    [TestFixture]
    public class NugetOperationTester
    {
        private INugetStep s1;
        private INugetStep s2;
        private INugetStep s3;
        private Solution theSolution;
        private INugetPlanBuilder thePlanBuilder;

        [SetUp]
        public void SetUp()
        {
            thePlanBuilder = MockRepository.GenerateStub<INugetPlanBuilder>();

            var r1 = new NugetPlanRequest { Dependency = new Dependency("d1")};
            var r2 = new NugetPlanRequest { Dependency = new Dependency("d2") };
            var r3 = new NugetPlanRequest { Dependency = new Dependency("d3") };

            s1 = MockRepository.GenerateStub<INugetStep>();
            s2 = MockRepository.GenerateStub<INugetStep>();
            s3 = MockRepository.GenerateStub<INugetStep>();

            thePlanBuilder.Stub(x => x.PlanFor(r1)).Return(new NugetPlan(s1));
            thePlanBuilder.Stub(x => x.PlanFor(r2)).Return(new NugetPlan(s2));
            thePlanBuilder.Stub(x => x.PlanFor(r3)).Return(new NugetPlan(s3));
            
            theSolution = new Solution();
            theSolution.UseBuilder(thePlanBuilder);
            theSolution.UseStorage(new StubNugetStorage());

            var input = new StubNugetOperationContext(r1, r2, r3);
            new NugetOperation { Solution = theSolution}.Execute(input, null);
        }

        [Test]
        public void executes_the_aggregate_plan()
        {
            s1.AssertWasCalled(x => x.Execute(null), o => o.IgnoreArguments());
            s2.AssertWasCalled(x => x.Execute(null), o => o.IgnoreArguments());
            s3.AssertWasCalled(x => x.Execute(null), o => o.IgnoreArguments());
        }
    }

    public class StubNugetOperationContext : SolutionInput, INugetOperationContext
    {
        private readonly IEnumerable<NugetPlanRequest> _requests;

        public StubNugetOperationContext(params NugetPlanRequest[] requests)
        {
            _requests = requests;
        }

        public IEnumerable<NugetPlanRequest> Requests(Solution solution)
        {
            return _requests;
        }
    }
}