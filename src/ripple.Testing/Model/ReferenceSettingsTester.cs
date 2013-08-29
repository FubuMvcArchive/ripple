using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class ReferenceSettingsTester
    {
        [Test]
        public void should_add_reference_to_assembly_with_no_ignore()
        {
            var settings = new ReferenceSettings();
            settings.ShouldAddReference(new Dependency("FubuCore"), "FubuCore").ShouldBeTrue();
        }

        [Test]
        public void should_not_add_reference_to_assembly_with_ignore()
        {
            var settings = new ReferenceSettings();
            settings.Ignore("FubuCore", "FubuCore.dll");
            settings.ShouldAddReference(new Dependency("FubuCore"), "FubuCore").ShouldBeFalse();
        }
    }
}