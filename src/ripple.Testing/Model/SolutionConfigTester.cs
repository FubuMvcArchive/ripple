using NUnit.Framework;
using ripple.Model;
using FubuTestingSupport;
using System.Linq;

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

        [Test]
        public void float_nuget()
        {
            var config = new SolutionConfig();
            config.FloatNuget("Nug1");

            config.ModeForNuget("Nug1").ShouldEqual(UpdateMode.Float);
            config.Floats.ShouldContain("Nug1");
        }

        [Test]
        public void lock_nuget()
        {
            var config = new SolutionConfig();
            config.FloatNuget("Nug1");
            config.LockNuget("Nug1");

            config.ModeForNuget("Nug1").ShouldEqual(UpdateMode.Locked);
            config.Floats.Any().ShouldBeFalse();
        }
    }
}