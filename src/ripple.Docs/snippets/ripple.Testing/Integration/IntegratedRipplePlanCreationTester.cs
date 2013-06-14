using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Local;
using ripple.Model;

namespace ripple.Testing.Integration
{
    [TestFixture]
    public class IntegratedRipplePlanCreationTester
    {
        private RipplePlanRequirements theRequirements;
		private SolutionGraphScenario theScenario;


		[SetUp]
		public void SetUp()
		{
			theScenario = SolutionGraphScenario.Create(scenario =>
			{
				scenario.Solution("Bottles", bottles =>
				{
					bottles.Publishes("Bottles", x => x.Assembly("Bottles.dll", "lib").DependsOn("FubuCore"));
					bottles.ProjectDependency("Bottles", "FubuCore");
				});

				// Defaults to "FubuCore.dll" targeting "lib"
				scenario.Solution("FubuCore", fubucore => fubucore.Publishes("FubuCore"));

				scenario.Solution("FubuLocalization", localization =>
				{
					localization.Publishes("FubuLocalization", x => x.Assembly("FubuLocalization.dll", "lib").DependsOn("FubuCore"));
					localization.ProjectDependency("FubuLocalization", "FubuCore");
				});

				scenario.Solution("FubuMVC", fubumvc =>
				{
					fubumvc.Publishes("FubuMVC.Core", x =>
					{
						x.Assembly("FubuMVC.Core.dll", "lib\\net40");
						x.DependsOn("Bottles");
						x.DependsOn("FubuCore");
						x.DependsOn("FubuLocalization");
						x.DependsOn("HtmlTags");
					});

					fubumvc.ProjectDependency("FubuMVC.Core", "Bottles");
					fubumvc.ProjectDependency("FubuMVC.Core", "FubuCore");
					fubumvc.ProjectDependency("FubuMVC.Core", "FubuLocalization");
					fubumvc.ProjectDependency("FubuMVC.Core", "HtmlTags");
				});

				scenario.Solution("FubuMVC.Core.View", views =>
				{
					views.Publishes("FubuMVC.Core.View", x => x.Assembly("FubuMVC.Core.View.dll", "lib\\net40").DependsOn("FubuMVC.Core"));

					views.ProjectDependency("FubuMVC.Core.View", "Bottles");
					views.ProjectDependency("FubuMVC.Core.View", "FubuCore");
					views.ProjectDependency("FubuMVC.Core.View", "FubuLocalization");
					views.ProjectDependency("FubuMVC.Core.View", "FubuMVC.Core");
					views.ProjectDependency("FubuMVC.Core.View", "HtmlTags");
				});

				scenario.Solution("FubuMVC.Core.UI", ui =>
				{
					ui.Publishes("FubuMVC.Core.UI", x => x.Assembly("FubuMVC.Core.UI.dll", "lib\\net40").DependsOn("FubuMVC.Core.View"));

					ui.ProjectDependency("FubuMVC.Core.UI", "Bottles");
					ui.ProjectDependency("FubuMVC.Core.UI", "FubuCore");
					ui.ProjectDependency("FubuMVC.Core.UI", "FubuLocalization");
					ui.ProjectDependency("FubuMVC.Core.UI", "FubuMVC.Core");
					ui.ProjectDependency("FubuMVC.Core.UI", "FubuMVC.Core.View");
					ui.ProjectDependency("FubuMVC.Core.UI", "HtmlTags");
				});

				scenario.Solution("HtmlTags", htmlTags => htmlTags.Publishes("HtmlTags", x => x.Assembly("HtmlTags.dll", "lib\\4.0")));
			});

			theRequirements = null;
		}

		[TearDown]
		public void TearDown()
		{
			theScenario.Cleanup();
		}

        private void theRippleStepsShouldBe(params Local.IRippleStep[] steps)
        {
            var plan = theRequirements.BuildPlan(theScenario.Graph);
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
			return new MoveExpression(theScenario.Graph, nugetName);
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
			return new BuildSolution(theScenario.Find(solutionName));
        }

        [Test]
        public void create_a_plan_for_only_two_projects()
        {
            theRequirements = new RipplePlanRequirements{
                From = "FubuCore",
                To = "Bottles"
            };

            theRippleStepsShouldBe(
                build("FubuCore"),
                move("FubuCore").To("Bottles"),
                build("Bottles")                
            );
        }

        // SAMPLE: RippleLocal
        [Test]
        public void create_a_plan_for_setting_from_and_to()
        {
            theRequirements = new RipplePlanRequirements{
                From = "FubuCore",
                To = "FubuMVC"
            };

            theRippleStepsShouldBe(
                build("FubuCore"),
                move("FubuCore").To("Bottles"),

                build("Bottles"), 
                
                move("FubuCore").To("FubuLocalization"),
                build("FubuLocalization"),

                move("Bottles").To("FubuMVC"),
                move("FubuCore").To("FubuMVC"),
                move("FubuLocalization").To("FubuMVC"),
				
                build("FubuMVC")
            );
        }
        // ENDSAMPLE

		[Test]
		public void create_a_plan_for_setting_from_and_to_with_no_build()
		{
			theRequirements = new RipplePlanRequirements
			{
				From = "FubuCore",
				To = "Bottles",
				SkipBuild = true
			};

			theRippleStepsShouldBe(
				build("FubuCore"),
				move("FubuCore").To("Bottles")
			);
		}

        [Test]
        public void create_a_direct_plan()
        {
            theRequirements = new RipplePlanRequirements
            {
                From = "FubuCore",
                To = "FubuMVC",
                Direct = true
            };

            theRippleStepsShouldBe(
                build("FubuCore"),
                move("FubuCore").To("FubuMVC"),

                build("FubuMVC")
            );
        }
    }
}