using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model.Conditions;

namespace ripple.Testing.Model.Conditions
{
    [TestFixture]
    public class DetectSingleSolutionTester
    {
        [Test]
        public void matches_when_a_single_sln_file_is_found()
        {
            using (var sandbox = DirectorySandbox.Create())
            {
                sandbox.CreateDirectory("src");
                sandbox.CreateFile("src", "Test.sln");

                new DetectSingleSolution()
                    .Matches(new FileSystem(), sandbox.Directory)
                    .ShouldBeTrue();
            }
        }

        [Test]
        public void no_match_when_multiple_sln_files_are_found()
        {
            using (var sandbox = DirectorySandbox.Create())
            {
                sandbox.CreateDirectory("src");
                sandbox.CreateFile("src", "Test1.sln");
                sandbox.CreateFile("src", "Test2.sln");

                new DetectSingleSolution()
                    .Matches(new FileSystem(), sandbox.Directory)
                    .ShouldBeFalse();
            }
        }
    }
}