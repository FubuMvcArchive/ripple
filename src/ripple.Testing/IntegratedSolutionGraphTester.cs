using System;
using System.Linq;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing
{
    [TestFixture]
    public class IntegratedSolutionGraphTester
    {
	    private SolutionGraphScenario theScenario;
        private SolutionGraphBuilder theBuilder;

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
		    });

            theBuilder = new SolutionGraphBuilder(new FileSystem());
        }

		[TestFixtureTearDown]
		public void TearDown()
		{
			theScenario.Cleanup();
		}

        [Test]
        public void was_able_to_find_published_assemblies_not_directly_in_lib()
        {
			var nuspec = theScenario.Graph.FindNugetSpec("HtmlTags");
        
            nuspec.PublishedAssemblies.Any().ShouldBeTrue();

            nuspec.PublishedAssemblies.Any(x => x.Name == "HtmlTags" && x.SubFolder == "lib/4.0");
        }

        [Test]
        public void can_read_from_a_ripple_folder_by_going_up_one()
        {
            theBuilder.ReadFrom(theScenario.Directory.AppendPath("FubuCore"))
                .AllSolutions.Select(x => x.Name).OrderBy(x => x)
                .ShouldHaveTheSameElementsAs("Bottles", "FubuCore", "FubuLocalization", "FubuMVC", "FubuMVC.Core.UI", "FubuMVC.Core.View", "HtmlTags");
        }

        [Test]
        public void can_read_from_a_root_get_folder()
        {
			theScenario.Graph
                .AllSolutions.Select(x => x.Name).OrderBy(x => x)
				.ShouldHaveTheSameElementsAs("Bottles", "FubuCore", "FubuLocalization", "FubuMVC", "FubuMVC.Core.UI", "FubuMVC.Core.View", "HtmlTags");
        }

        [Test]
        public void solution_graph_can_find_nuget_specs()
        {
            theScenario.Graph.FindNugetSpec("FubuCore").ShouldNotBeNull();
			theScenario.Graph.FindNugetSpec("FubuLocalization").ShouldNotBeNull();
            theScenario.Graph.FindNugetSpec("Bottles").ShouldNotBeNull();
            theScenario.Graph.FindNugetSpec("FubuMVC.Core.View").ShouldNotBeNull();
			theScenario.Graph.FindNugetSpec("FubuMVC.Core.UI").ShouldNotBeNull();
            theScenario.Graph.FindNugetSpec("FubuMVC.Core").ShouldNotBeNull();
			theScenario.Graph.FindNugetSpec("HtmlTags").ShouldNotBeNull();
        }

        [Test]
        public void solution_graph_happily_returns_null_for_nugets_that_are_not_published_from_any_of_the_contained_projects()
        {
            theScenario.Graph.FindNugetSpec("CommonServiceLocator").ShouldBeNull();
            theScenario.Graph.FindNugetSpec("structuremap").ShouldBeNull();
        }

        [Test]
        public void can_return_the_solutions_in_dependency_order()
        {
            var names = theScenario.Graph.AllSolutions.Select(x => x.Name);
            names.ShouldHaveTheSameElementsAs(
				"FubuCore",
				"HtmlTags",
				"Bottles",
				"FubuLocalization",
				"FubuMVC",
				"FubuMVC.Core.View",
				"FubuMVC.Core.UI"
            );
        }

        [Test]
        public void has_read_all_the_published_nuget_specs()
        {
            theScenario.Graph.AllNugets().Select(x => x.Name).ShouldHaveTheSameElementsAs(
				"Bottles",
				"FubuCore",
				"FubuLocalization",
				"FubuMVC.Core",
				"FubuMVC.Core.UI",
				"FubuMVC.Core.View",
				"HtmlTags"
            );

        }


        [Test]
        public void find_nuget_spec_works()
        {
            theScenario.Graph.FindNugetSpec("HtmlTags").ShouldNotBeNull();
        }


        [Test]
        public void solutions_list_their_dependencies()
        {
            theScenario.Graph["FubuCore"].SolutionDependencies().Any().ShouldBeFalse();
            theScenario.Graph["Bottles"].SolutionDependencies().Select(x => x.Name).ShouldHaveTheSameElementsAs("FubuCore");
			theScenario.Graph["FubuLocalization"].SolutionDependencies().Select(x => x.Name).ShouldHaveTheSameElementsAs("FubuCore");
            theScenario.Graph["FubuMVC"].SolutionDependencies().Select(x => x.Name).ShouldHaveTheSameElementsAs("Bottles", "FubuCore", "FubuLocalization", "HtmlTags");
			theScenario.Graph["FubuMVC.Core.UI"].SolutionDependencies().Select(x => x.Name).ShouldHaveTheSameElementsAs("Bottles", "FubuCore", "FubuLocalization", "FubuMVC", "FubuMVC.Core.View", "HtmlTags");
			theScenario.Graph["FubuMVC.Core.View"].SolutionDependencies().Select(x => x.Name).ShouldHaveTheSameElementsAs("Bottles", "FubuCore", "FubuLocalization", "FubuMVC", "HtmlTags");

        }
    }
}