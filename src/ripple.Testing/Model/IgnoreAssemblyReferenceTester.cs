using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class IgnoreAssemblyReferenceTester
    {
        [Test]
        public void matches_dependency_and_assembly()
        {
            var reference = new IgnoreAssemblyReference {Package = "Test", Assembly = "Test"};
            reference.Matches(new Dependency("Test"), "Test").ShouldBeTrue();
        }

        [Test]
        public void matches_dependency_and_assembly_with_file_extension()
        {
            var reference = new IgnoreAssemblyReference { Package = "Test", Assembly = "Test.dll" };
            reference.Matches(new Dependency("Test"), "Test").ShouldBeTrue();
        }

        [Test]
        public void no_match()
        {
            var reference = new IgnoreAssemblyReference { Package = "Test", Assembly = "Test2.dll" };
            reference.Matches(new Dependency("Test"), "Test").ShouldBeFalse();
        }
    }
}