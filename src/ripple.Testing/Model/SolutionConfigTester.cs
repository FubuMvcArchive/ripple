using NUnit.Framework;
using ripple.Model;
using FubuTestingSupport;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class SolutionConfigTester
    {
        [Test]
        public void mode_for_nuget_that_does_not_match_floats()
        {
            var config = new SolutionConfig();
            config.ModeForNuget("anything").ShouldEqual(UpdateMode.Locked);
        }

        [Test]
        public void mode_for_nuget_that_matches_floats()
        {
            var config = new SolutionConfig();
            config.Floats = new string[]{"FubuCore"};

            config.ModeForNuget("FubuCore").ShouldEqual(UpdateMode.Float);
        }
    }
}