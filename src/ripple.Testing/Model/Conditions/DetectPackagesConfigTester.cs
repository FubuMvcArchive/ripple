using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model.Conditions;

namespace ripple.Testing.Model.Conditions
{
    [TestFixture]
    public class DetectPackagesConfigTester
    {
        [Test]
        public void matches_when_a_packages_config_is_found()
        {
            using (var sandbox = DirectorySandbox.Create())
            {
                sandbox.CreateDirectory("src", "MyProject");
                sandbox.CreateFile("src", "MyProject", "packages.config");

                new DetectPackagesConfig()
                    .Matches(new FileSystem(), sandbox.Directory)
                    .ShouldBeTrue();
            }
        }

        [Test]
        public void no_match_when_no_packages_config_is_found()
        {
            using (var sandbox = DirectorySandbox.Create())
            {
                sandbox.CreateDirectory("src", "MyProject");
                sandbox.CreateFile("src", "MyProject", "something-else.config");

                new DetectPackagesConfig()
                    .Matches(new FileSystem(), sandbox.Directory)
                    .ShouldBeFalse();
            }
        }
    }
}