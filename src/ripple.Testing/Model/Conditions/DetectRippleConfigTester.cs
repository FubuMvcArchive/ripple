using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model.Conditions;

namespace ripple.Testing.Model.Conditions
{
    [TestFixture]
    public class DetectRippleConfigTester
    {
        [Test]
        public void matches_when_ripple_config_is_found()
        {
            using (var sandbox = DirectorySandbox.Create())
            {
                sandbox.StopAtParent();
                sandbox.CreateFile("ripple.config");

                new DetectRippleConfig()
                    .Matches(new FileSystem(), sandbox.Directory)
                    .ShouldBeTrue();
            }
        }

        [Test]
        public void no_match_when_no_ripple_config_is_found()
        {
            using (var sandbox = DirectorySandbox.Create())
            {
                sandbox.StopAtParent();
                sandbox.CreateDirectory("src", "MyProject");
                sandbox.CreateFile("src", "MyProject", "something-else.config");

                new DetectRippleConfig()
                    .Matches(new FileSystem(), sandbox.Directory)
                    .ShouldBeFalse();
            }
        }
    }
}