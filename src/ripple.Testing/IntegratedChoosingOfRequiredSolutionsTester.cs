using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Local;
using ripple.Model;

namespace ripple.Testing
{
    [TestFixture]
    public class IntegratedChoosingOfRequiredSolutionsTester
    {
        private RipplePlanRequirements theRequirements;
		private SolutionGraphScenario theScenario;
		private SolutionGraphBuilder theBuilder;
		private SolutionGraph theGraph;


		[TestFixtureSetUp]
		public void FixtureSetUp()
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

				scenario.Solution("Validation", validation =>
				{
					validation.Publishes("FubuValidation", x => x.Assembly("FubuValidation.dll", "lib\\net40").DependsOn("FubuCore"));
					validation.ProjectDependency("FubuValidation", "FubuCore");
				});
			});

			theBuilder = new SolutionGraphBuilder(new FileSystem());

			theGraph = theBuilder.ReadFrom(theScenario.Directory);
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			theScenario.Cleanup();
		}

        [SetUp]
        public void SetUp()
        {
            theRequirements = null;
        }

        private void theSolutionsShouldBe(params string[] names)
        {
            var theNames = theRequirements.SelectSolutions(theGraph).Select(x => x.Name);
            try
            {
                theNames
                    .ShouldHaveTheSameElementsAs(names);
            }
            catch (Exception)
            {
                Debug.WriteLine("Expected:  " + names.Join(", "));
                Debug.WriteLine("Actual:  " + theNames.Join(", "));
                throw;
            }
        }

        [Test]
        public void get_them_all()
        {
            theRequirements = new RipplePlanRequirements();
            theSolutionsShouldBe("FubuCore",
                                 "HtmlTags",
								 "Bottles",
								 "FubuLocalization",
								 "Validation",
                                 "FubuMVC",
                                 "FubuMVC.Core.View",
								 "FubuMVC.Core.UI");
        }

        [Test]
        public void start_at_a_project()
        {
            theRequirements = new RipplePlanRequirements
                              {
                                  From = "Bottles"
                              };

            theSolutionsShouldBe("Bottles", "FubuMVC", "FubuMVC.Core.View", "FubuMVC.Core.UI");
        }

        [Test]
        public void set_from_and_to()
        {
            theRequirements = new RipplePlanRequirements{
                From = "HtmlTags",
                To = "FubuMVC"
            };

            theSolutionsShouldBe("HtmlTags", "FubuMVC");
        }

        [Test]
        public void set_from_and_to_2()
        {
            theRequirements = new RipplePlanRequirements
                              {
                                  From = "FubuCore",
                                  To = "FubuMVC"
                              };

            theSolutionsShouldBe("FubuCore", "Bottles", "FubuLocalization", "FubuMVC");
        }

        [Test]
        public void set_from_and_to_3()
        {
            theRequirements = new RipplePlanRequirements
                              {
                                  From = "FubuCore",
                                  To = "FubuMVC.Core.View"
                              };

            theSolutionsShouldBe("FubuCore", "Bottles", "FubuLocalization", "FubuMVC", "FubuMVC.Core.View");
        }

        [Test]
        public void set_from_and_to_4()
        {
            theRequirements = new RipplePlanRequirements
                              {
                                  From = "FubuCore",
                                  To = "FubuMVC.Core.UI"
                              };

			theSolutionsShouldBe("FubuCore", "Bottles", "FubuLocalization", "FubuMVC", "FubuMVC.Core.View", "FubuMVC.Core.UI");
        }

        [Test]
        public void trying_direct_option_without_a_To()
        {
            Exception<InvalidOperationException>.ShouldBeThrownBy(() =>
            {
                theRequirements = new RipplePlanRequirements(){
                    From = "FubuCore",
                    To = null,
                    Direct = true
                };

                theRequirements.SelectSolutions(theGraph);
            });
        }

        [Test]
        public void trying_direct_option_without_a_From()
        {
            Exception<InvalidOperationException>.ShouldBeThrownBy(() =>
            {
                theRequirements = new RipplePlanRequirements()
                                  {
                                      From = null,
                                      To = "FubuMVC",
                                      Direct = true
                                  };

                theRequirements.SelectSolutions(theGraph);
            });
        }

        [Test]
        public void throw_invalid_solution_if_to_does_not_exist()
        {
            Exception<InvalidSolutionException>.ShouldBeThrownBy(() =>
            {
                theRequirements = new RipplePlanRequirements()
                                  {
                                      From = null,
                                      To = "junk",
                                  };

                theRequirements.SelectSolutions(theGraph);
            });
        }

        [Test]
        public void throw_invalid_solution_if_from_does_not_exist()
        {
            Exception<InvalidSolutionException>.ShouldBeThrownBy(() =>
            {
                theRequirements = new RipplePlanRequirements()
                                  {
                                      From = "junk",
                                  };

                theRequirements.SelectSolutions(theGraph);
            });
        }

        [Test]
        public void direct_skips_intermediate_steps()
        {
            theRequirements = new RipplePlanRequirements
                              {
                                  From = "FubuCore",
                                  To = "FubuMVC",
                                  Direct = true
                              };

            theSolutionsShouldBe("FubuCore", "FubuMVC");
        }


        [Test]
        public void start_at_a_project_2()
        {
            theRequirements = new RipplePlanRequirements
                              {
                                  From = "HtmlTags"
                              };

            theSolutionsShouldBe("HtmlTags", "FubuMVC", "FubuMVC.Core.View", "FubuMVC.Core.UI");
        }

        [Test]
        public void start_at_a_project_skips_projects_that_are_not_dependent_on_the_from()
        {
            theRequirements = new RipplePlanRequirements
                              {
                                  From = "Validation"
                              };

            theSolutionsShouldBe("Validation");
        }

        [Test]
        public void stop_at_a_to_project()
        {
            theRequirements = new RipplePlanRequirements{
                To = "Bottles"
            };

            theSolutionsShouldBe("FubuCore", "Bottles");
        }

        [Test]
        public void stop_at_a_project_2()
        {
            theRequirements = new RipplePlanRequirements{
                To = "FubuMVC"
            };

            theSolutionsShouldBe("FubuCore", "HtmlTags", "Bottles", "FubuLocalization", "FubuMVC");
        }
    }
}