using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;

namespace ripple.Testing
{
    [TestFixture]
    public class PackageExtensionsTester
    {
        [Test]
        public void uses_max_version_if_available()
        {
            var package = new StubPackage("Bottles", "0.1.1.1");
            package.DependsOn("FubuCore").Min("0.9.9.9999").Max("1.0.0.0");

            var fubucore = package.ImmediateDependencies().Single();
            fubucore.Name.ShouldEqual("FubuCore");
            fubucore.Version.ShouldEqual("1.0.0.0");
        }

        [Test]
        public void uses_min_version_if_max_version_is_unavailable()
        {
            var package = new StubPackage("Bottles", "0.1.1.1");
            package.DependsOn("FubuCore").Min("0.9.9.9999");

            var fubucore = package.ImmediateDependencies().Single();
            fubucore.Name.ShouldEqual("FubuCore");
            fubucore.Version.ShouldEqual("0.9.9.9999");
        }

        [Test]
        public void no_version_if_version_spec_is_null()
        {
            // will get treated as a float during install
            var package = new StubPackage("Bottles", "0.1.1.1");
            package.DependsOn("FubuCore");

            var fubucore = package.ImmediateDependencies().Single();
            fubucore.Name.ShouldEqual("FubuCore");
            fubucore.Version.ShouldBeEmpty();
        }
    }
}