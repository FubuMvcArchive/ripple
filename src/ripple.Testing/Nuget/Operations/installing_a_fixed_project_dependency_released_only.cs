using System;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget.Operations
{
    [TestFixture]
    public class installing_a_fixed_project_dependency_released_only : NugetOperationContext
    {
        private SolutionGraphScenario theScenario;
        private Solution theSolution;
        private NugetPlan thePlan;
        private NugetPlanBuilder theBuilder;

        [SetUp]
        public void SetUp()
        {
            FeedScenario.Create(scenario =>
            {
                scenario.For(Feed.NuGetV2)
                    .Add("FubuCore", "1.2.0.0-alpha");
            });

            theScenario = SolutionGraphScenario.Create(scenario => scenario.Solution("Test"));

            theSolution = theScenario.Find("Test");

            theBuilder = new NugetPlanBuilder();
           
        }

        [TearDown]
        public void TearDown()
        {
            theScenario.Cleanup();
            FeedRegistry.Reset();
        }

        [Test]
        public void nuget_is_not_found()
        {
            Exception<RippleFatalError>
            .ShouldBeThrownBy(() => theBuilder.PlanFor(new NugetPlanRequest
            {
                Solution = theSolution,
                Dependency = new Dependency("FubuCore", "1.2.0.0", UpdateMode.Fixed),
                Operation = OperationType.Install,
                Project = "Test"
            })).Message.ShouldContain("Could not find FubuCore");
        }
    }
}