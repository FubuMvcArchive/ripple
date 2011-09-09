using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing
{
    [TestFixture]
    public class IntegratedSolutionDependencyQueryTester
    {
        private SolutionGraphBuilder theBuilder;
        private SolutionGraph theGraph;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            DataMother.CreateDataFolder();
            theBuilder = new SolutionGraphBuilder(new FileSystem());

            theGraph = theBuilder.ReadFrom("data");
        }

        [Test]
        public void depends_on_positive()
        {
            theGraph["bottles"].DependsOn(theGraph["fubucore"]).ShouldBeTrue();
            theGraph["fubumvc"].DependsOn(theGraph["fubucore"]).ShouldBeTrue();
            theGraph["fubumvc"].DependsOn(theGraph["bottles"]).ShouldBeTrue();
            theGraph["fastpack"].DependsOn(theGraph["fubucore"]).ShouldBeTrue();
            theGraph["fastpack"].DependsOn(theGraph["htmltags"]).ShouldBeTrue();
        }

        [Test]
        public void depends_on_negative()
        {
            theGraph["fubucore"].DependsOn(theGraph["fubumvc"]).ShouldBeFalse();
            theGraph["fubucore"].DependsOn(theGraph["htmltags"]).ShouldBeFalse();
            theGraph["fubucore"].DependsOn(theGraph["validation"]).ShouldBeFalse();
            theGraph["fubumvc"].DependsOn(theGraph["validation"]).ShouldBeFalse();
        }
    }
}