using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model.Versioning;

namespace ripple.Testing.Model.Versioning
{
    [TestFixture]
    public class InequalityRuleTester
    {
        [Test]
        public void matches_when_versions_are_not_equal()
        {
            var configured = new SemanticVersion(1, 1, 0, 1);
            var target = new SemanticVersion(1, 1, 1, 1);

            new InequalityRule(configured).Matches(target).ShouldBeTrue();
        }

        [Test]
        public void no_match_when_versions_are_equal()
        {
            var configured = new SemanticVersion(1, 1, 1, 1);
            var target = new SemanticVersion(1, 1, 1, 1);

            new InequalityRule(configured).Matches(target).ShouldBeFalse();
        }
    }
}