using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Nuget
{
    [TestFixture]
    public class LocalDependenciesTester
    {
        private StubNugetFile n1;
        private StubNugetFile n2;
        private StubNugetFile n3;

        private LocalDependencies theDependencies;

        [SetUp]
        public void SetUp()
        {
            n1 = new StubNugetFile(new Dependency("FubuCore", "1.0.0.1"));
            n2 = new StubNugetFile(new Dependency("Bottles", "1.2.0.1"));
            n3 = new StubNugetFile(new Dependency("FubuMVC.Core", "1.1.0.1"));

            theDependencies = new LocalDependencies(new[] { n1, n2, n3 });
        }

        [Test]
        public void any()
        {
            theDependencies.Any().ShouldBeTrue();
        }

        [Test]
        public void any_negative()
        {
            new LocalDependencies(new INugetFile[0]).Any().ShouldBeFalse();
        }

        [Test]
        public void gets_the_dependency()
        {
            theDependencies.Get("FubuCore").ShouldEqual(n1);
            theDependencies.Get("FubuMVC.Core").ShouldEqual(n3);
            theDependencies.Get("Bottles").ShouldEqual(n2);
        }

        [Test]
        public void has()
        {
            theDependencies.Has("FubuCore").ShouldBeTrue();
            theDependencies.Has("Bottles").ShouldBeTrue();
            theDependencies.Has("FubuMVC.Core").ShouldBeTrue();
        }

        [Test]
        public void has_negative()
        {
            theDependencies.Has("FubuMVC.Json").ShouldBeFalse();
        }

        [Test]
        public void should_restore_when_missing()
        {
            theDependencies.ShouldRestore(new Dependency("FubuLocalization")).ShouldBeTrue();
        }

        [Test]
        public void should_not_restore_when_exists()
        {
            theDependencies.ShouldRestore(new Dependency("FubuCore")).ShouldBeFalse();
        }

        [Test]
        public void should_force_restore_when_versions_do_not_match()
        {
            // local copy is 1.0.0.1
            theDependencies.ShouldRestore(new Dependency("FubuCore", "1.1.0.0"), true).ShouldBeTrue();
        }

        [Test]
        public void should_force_restore_for_a_float()
        {
            theDependencies.ShouldRestore(Dependency.FloatFor("FubuCore"), true).ShouldBeTrue();
        }

        [Test]
        public void should_restore_when_min_version_is_higher_than_local()
        {
            theDependencies.ShouldRestore(new Dependency("FubuCore", "1.1.0.2")).ShouldBeTrue();
        }
    }
}