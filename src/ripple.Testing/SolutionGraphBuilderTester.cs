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
    public class SolutionGraphBuilderTester
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
    }
}