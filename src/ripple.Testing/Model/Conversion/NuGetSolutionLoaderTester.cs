using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Model.Conditions;
using ripple.Model.Conversion;

namespace ripple.Testing.Model.Conversion
{
    [TestFixture]
    public class NuGetSolutionLoaderTester
    {
        private NuGetSolutionLoader theLoader { get { return new NuGetSolutionLoader(); } }

        [Test]
        public void builds_the_condition()
        {
            theLoader
                .Condition
                .As<CompositeDirectoryCondition>()
                .Conditions
                .ShouldHaveTheSameElementsAs(
                    new DetectPackagesConfig(),
                    new DetectSingleSolution(),
                    new NotDirectoryCondition(new DetectRippleConfig()),
                    new NotDirectoryCondition(new DetectRippleDependencies())
                );
        }

        [Test]
        public void loads_the_solution_name()
        {
            using (var sandbox = DirectorySandbox.Create())
            {
                sandbox.CreateDirectory("SourceFolder");
                sandbox.CreateFile("SourceFolder", "Test.sln");

                var solution = theLoader.LoadFrom(new FileSystem(), sandbox.Directory);
                solution.Feeds.ShouldHaveTheSameElementsAs(Feed.NuGetV2);
                solution.Name.ShouldEqual("Test");
            }
        }
    }
}