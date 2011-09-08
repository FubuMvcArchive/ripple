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
    public class IntegratedSolutionGraphTester
    {
        private SolutionGraphBuilder theBuilder;
        private Lazy<SolutionGraph> theGraph;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            DataMother.CreateDataFolder();
            theBuilder = new SolutionGraphBuilder(new FileSystem());

            theGraph = new Lazy<SolutionGraph>(() => theBuilder.ReadFrom("data"));
        }

        [Test]
        public void can_read_from_a_ripple_folder_by_going_up_one()
        {
            theBuilder.ReadFrom("data".AppendPath("fubucore"))
                .AllSolutions.Select(x => x.Name).OrderBy(x => x)
                .ShouldHaveTheSameElementsAs("bottles", "fastpack", "fubucore", "fubumvc", "htmltags", "validation");
        }

        [Test]
        public void can_read_from_a_root_get_folder()
        {
            theGraph.Value
                .AllSolutions.Select(x => x.Name).OrderBy(x => x)
                .ShouldHaveTheSameElementsAs("bottles", "fastpack", "fubucore", "fubumvc", "htmltags", "validation");
        }

        [Test]
        public void solution_graph_can_find_nuget_specs()
        {
            theGraph.Value.FindNugetSpec("FubuCore").ShouldNotBeNull();
            theGraph.Value.FindNugetSpec("Bottles").ShouldNotBeNull();
            theGraph.Value.FindNugetSpec("FubuMVC.References").ShouldNotBeNull();
            theGraph.Value.FindNugetSpec("FubuMVC").ShouldNotBeNull();
        }

        [Test]
        public void solution_graph_happily_returns_null_for_nugets_that_are_not_published_from_any_of_the_contained_projects()
        {
            theGraph.Value.FindNugetSpec("CommonServiceLocator").ShouldBeNull();
            theGraph.Value.FindNugetSpec("structuremap").ShouldBeNull();
        }

        [Test]
        public void can_return_the_solutions_in_dependency_order()
        {
            var names = theGraph.Value.AllSolutions.Select(x => x.Name);
            names.ShouldHaveTheSameElementsAs(
"fubucore",
"htmltags",
"validation",
"bottles",
"fubumvc",
"fastpack"
                );
        }

        [Test]
        public void has_read_all_the_published_nuget_specs()
        {
            theGraph.Value.AllNugets().Select(x => x.Name).ShouldHaveTheSameElementsAs(
"Bottles.Deployers.IIS",
"Bottles.Deployers.TopShelf",
"Bottles.Deployment",
"Bottles.Host.Packaging",
"Bottles",
"Bottles.Tools",
"FubuMVC.FastPack",
"FubuCore",
"FubuLocalization",
"FubuTestingSupport",
"FubuMVC.Diagnostics",
"FubuMVC",
"FubuMVC.References",
"FubuMVC.Spark",
"FubuMVC.WebForms",
"HtmlTags",
"FubuValidation"

            
                );

        }


        [Test]
        public void find_nuget_spec_works()
        {
            theGraph.Value.FindNugetSpec("HtmlTags").ShouldNotBeNull();
        }


        [Test]
        public void solutions_list_their_dependencies()
        {
            theGraph.Value["fubucore"].Dependencies().Any().ShouldBeFalse();
            theGraph.Value["bottles"].Dependencies().Select(x => x.Name).ShouldHaveTheSameElementsAs("fubucore", "htmltags");
            theGraph.Value["fubumvc"].Dependencies().Select(x => x.Name).ShouldHaveTheSameElementsAs("bottles", "fubucore", "htmltags");
            theGraph.Value["fastpack"].Dependencies().Select(x => x.Name).ShouldHaveTheSameElementsAs("bottles", "fubucore", "fubumvc", "htmltags", "validation");

        }
    }
}