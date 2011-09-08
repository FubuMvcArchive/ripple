using System;
using System.Collections.Generic;
using System.Diagnostics;
using FubuCore;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace ripple.Testing
{
    [TestFixture]
    public class IntegratedRipplePlanCreationTester
    {
        private SolutionGraphBuilder theBuilder;
        private SolutionGraph theGraph;
        private RipplePlanRequirements theRequirements;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            DataMother.CreateDataFolder();
            theBuilder = new SolutionGraphBuilder(new FileSystem());

            theGraph = theBuilder.ReadFrom("data");
        }

        [SetUp]
        public void SetUp()
        {
            theRequirements = null;
        }

        private void theRippleStepsShouldBe(params IRippleStep[] steps)
        {
            var plan = theRequirements.BuildPlan(theGraph);
            try
            {
                plan.ShouldHaveTheSameElementsAs(steps);
            }
            catch (Exception)
            {
                Debug.WriteLine("Expected:  " + steps.Select(x => x.ToString()).Join(", "));
                Debug.WriteLine("Actual:    " + plan.Select(x => x.ToString()).Join(", "));
                throw;
            }
        }

        private MoveExpression move(string nugetName)
        {
            return new MoveExpression(theGraph, nugetName);
        }

        private class MoveExpression
        {
            private readonly SolutionGraph _solutionGraph;
            private readonly string _nugetName;

            public MoveExpression(SolutionGraph solutionGraph, string nugetName)
            {
                _solutionGraph = solutionGraph;
                _nugetName = nugetName;
            }

            public MoveNugetAssemblies To(string solutionName)
            {
                return new MoveNugetAssemblies(_solutionGraph.FindNugetSpec(_nugetName), _solutionGraph[solutionName]);
            }
        }

        private BuildSolution build(string solutionName)
        {
            return new BuildSolution(theGraph[solutionName]);
        }



        [Test]
        public void create_a_plan_for_only_two_projects()
        {
            theRequirements = new RipplePlanRequirements{
                From = "fubucore",
                To = "bottles"
            };

            theRippleStepsShouldBe(
                build("fubucore"),
                move("FubuCore").To("bottles"),
                move("FubuTestingSupport").To("bottles"),
                build("bottles")                
                );
        }

        [Test]
        public void create_a_plan_for_setting_from_and_to()
        {
            theRequirements = new RipplePlanRequirements{
                From = "fubucore",
                To = "fubumvc"
            };

            theRippleStepsShouldBe(
                build("fubucore"),
                move("FubuCore").To("bottles"),
                move("FubuTestingSupport").To("bottles"),

                build("bottles"), 
                move("Bottles").To("fubumvc"),
                move("Bottles.Deployers.IIS").To("fubumvc"),
                move("Bottles.Deployment").To("fubumvc"),
                move("FubuCore").To("fubumvc"),
                move("FubuLocalization").To("fubumvc"),
                move("FubuTestingSupport").To("fubumvc"),
                
                build("fubumvc")                
                );
        }

        [Test]
        public void create_a_direct_plan()
        {
            theRequirements = new RipplePlanRequirements
            {
                From = "fubucore",
                To = "fubumvc",
                Direct = true
            };

            theRippleStepsShouldBe(
                build("fubucore"),
                move("FubuCore").To("fubumvc"),
                move("FubuLocalization").To("fubumvc"),
                move("FubuTestingSupport").To("fubumvc"),
                build("fubumvc")
                );
        }
    }
}