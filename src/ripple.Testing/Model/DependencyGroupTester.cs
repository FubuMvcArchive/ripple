using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class DependencyGroupTester
    {
        [Test]
        public void has_dependency_positive()
        {
            var group = new DependencyGroup();
            group.Add(new GroupedDependency("FubuCore"));

            group.Has("FubuCore").ShouldBeTrue();
        }

        [Test]
        public void has_dependency_case_insensitive()
        {
            var group = new DependencyGroup();
            group.Add(new GroupedDependency("FubuCore"));

            group.Has("fubuCore").ShouldBeTrue();
        }

        [Test]
        public void has_dependency_negative()
        {
            var group = new DependencyGroup();
            group.Add(new GroupedDependency("FubuCore"));

            group.Has("Bottles").ShouldBeFalse();
        }
    }
}