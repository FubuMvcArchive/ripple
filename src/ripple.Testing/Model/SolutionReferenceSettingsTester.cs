using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class SolutionReferenceSettingsTester
    {
        [Test]
        public void should_add_reference_to_assembly_with_no_ignore()
        {
            var solution = Solution.Empty();
            solution.ShouldAddReference(new Dependency("FubuCore"), "FubuCore").ShouldBeTrue();
        }

        [Test]
        public void should_not_add_reference_to_assembly_with_ignore()
        {
            var solution = Solution.Empty();
            solution.Ignore("FubuCore", "FubuCore.dll");
            solution.ShouldAddReference(new Dependency("FubuCore"), "FubuCore").ShouldBeFalse();
        }
    }
}