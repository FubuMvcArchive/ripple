using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class NuspecSettingsTester
    {
        [Test]
        public void defaults()
        {
            var settings = new NuspecSettings();
            settings.Float.ShouldEqual(VersionConstraint.DefaultFloat);
            settings.Fixed.ShouldEqual(VersionConstraint.DefaultFixed);
        }

        [Test]
        public void constraint_for_fixed()
        {
            var settings = new NuspecSettings();
            settings.ConstraintFor(UpdateMode.Fixed).ShouldEqual(settings.Fixed);
        }

        [Test]
        public void constraint_for_float()
        {
            var settings = new NuspecSettings();
            settings.ConstraintFor(UpdateMode.Float).ShouldEqual(settings.Float);
        }
    }
}