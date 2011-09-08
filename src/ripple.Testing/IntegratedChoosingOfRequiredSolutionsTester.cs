using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace ripple.Testing
{
    [TestFixture]
    public class IntegratedChoosingOfRequiredSolutionsTester
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
            theSolutionsShouldBe("fubucore",
                                 "htmltags",
                                 "validation",
                                 "bottles",
                                 "fubumvc",
                                 "fastpack");
        }

        [Test]
        public void start_at_a_project()
        {
            theRequirements = new RipplePlanRequirements
                              {
                                  From = "bottles"
                              };

            theSolutionsShouldBe("bottles", "fubumvc", "fastpack");
        }

        [Test]
        public void set_from_and_to()
        {
            theRequirements = new RipplePlanRequirements{
                From = "htmltags",
                To = "fastpack"
            };

            theSolutionsShouldBe("htmltags", "bottles", "fubumvc", "fastpack");
        }

        [Test]
        public void set_from_and_to_2()
        {
            theRequirements = new RipplePlanRequirements
                              {
                                  From = "bottles",
                                  To = "fastpack"
                              };

            theSolutionsShouldBe("bottles", "fubumvc", "fastpack");
        }

        [Test]
        public void set_from_and_to_3()
        {
            theRequirements = new RipplePlanRequirements
                              {
                                  From = "fubucore",
                                  To = "fastpack"
                              };

            theSolutionsShouldBe("fubucore", "bottles", "fubumvc", "fastpack");
        }

        [Test]
        public void set_from_and_to_4()
        {
            theRequirements = new RipplePlanRequirements
                              {
                                  From = "fubucore",
                                  To = "fubumvc"
                              };

            theSolutionsShouldBe("fubucore", "bottles", "fubumvc");
        }

        [Test]
        public void trying_direct_option_without_a_To()
        {
            Exception<InvalidOperationException>.ShouldBeThrownBy(() =>
            {
                theRequirements = new RipplePlanRequirements(){
                    From = "fubucore",
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
                                      To = "fubumvc",
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
                                  From = "fubucore",
                                  To = "fubumvc",
                                  Direct = true
                              };

            theSolutionsShouldBe("fubucore", "fubumvc");
        }


        [Test]
        public void start_at_a_project_2()
        {
            theRequirements = new RipplePlanRequirements
                              {
                                  From = "htmltags"
                              };

            theSolutionsShouldBe("htmltags", "bottles", "fubumvc", "fastpack");
        }

        [Test]
        public void start_at_a_project_skips_projects_that_are_not_dependent_on_the_from()
        {
            theRequirements = new RipplePlanRequirements
                              {
                                  From = "validation"
                              };

            theSolutionsShouldBe("validation", "fastpack");
        }

        [Test]
        public void stop_at_a_to_project()
        {
            theRequirements = new RipplePlanRequirements{
                To = "bottles"
            };

            theSolutionsShouldBe("fubucore", "htmltags", "bottles");
        }

        [Test]
        public void stop_at_a_project_2()
        {
            theRequirements = new RipplePlanRequirements{
                To = "fubumvc"
            };

            theGraph["fubumvc"].DependsOn(theGraph["bottles"]).ShouldBeTrue();

            theSolutionsShouldBe("fubucore", "htmltags", "bottles", "fubumvc");
        }

        [Test]
        public void stop_at_a_project_3()
        {
            theRequirements = new RipplePlanRequirements
                              {
                                  To = "fastpack"
                              };

            theSolutionsShouldBe("fubucore", "htmltags", "validation", "bottles", "fubumvc", "fastpack");
        }
    }
}