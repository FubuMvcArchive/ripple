using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class RestoreSettingsTester
    {
        private RestoreSettings theSettings;

        [SetUp]
        public void SetUp()
        {
            theSettings = new RestoreSettings();
        }

        [Test]
        public void do_not_force_anything()
        {
            theSettings.ShouldForce(new Dependency("FubuCore")).ShouldBeFalse();
        }

        [Test]
        public void force_all()
        {
            theSettings.ForceAll();

            theSettings.ShouldForce(new Dependency("FubuCore")).ShouldBeTrue();
            theSettings.ShouldForce(null).ShouldBeTrue();
        }

        [Test]
        public void force_multiple_dependencies()
        {
            theSettings.Force("FubuCore");
            theSettings.Force("Bottles");

            theSettings.ShouldForce(new Dependency("FubuCore")).ShouldBeTrue();
            theSettings.ShouldForce(new Dependency("Bottles")).ShouldBeTrue();
        }

        [Test]
        public void force_a_specific_dependency()
        {
            theSettings.Force("FubuCore");

            theSettings.ShouldForce(new Dependency("FubuCore")).ShouldBeTrue();
            theSettings.ShouldForce(new Dependency("FubuCore", "1.0.0.0")).ShouldBeTrue();
            theSettings.ShouldForce(new Dependency("FubuCore", "1.2.0.0")).ShouldBeTrue();
        }

        [Test]
        public void force_a_specific_dependency_negative()
        {
            theSettings.Force("FubuCore");
            theSettings.ShouldForce(new Dependency("Bottles")).ShouldBeFalse();
        }

        [Test]
        public void force_all_then_force_a_specific_dependency()
        {
            theSettings.ForceAll();
            theSettings.Force("FubuCore");

            theSettings.ShouldForce(new Dependency("FubuCore")).ShouldBeTrue();
            theSettings.ShouldForce(new Dependency("Bottles")).ShouldBeFalse();
        }
    }
}