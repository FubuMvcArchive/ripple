using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model.Versioning;

namespace ripple.Testing.Model.Versioning
{
    [TestFixture]
    public class GreaterRuleTester
    {
        [Test]
        public void matches_when_target_version_is_greater_than_configured()
        {
            var configured = new SemanticVersion(1, 1, 1, 1);
            var target = new SemanticVersion(2, 0, 0, 0);

            new GreaterRule(configured).Matches(target).ShouldBeTrue();
        }

        [Test]
        public void no_match_when_versions_are_equal()
        {
            var configured = new SemanticVersion(1, 1, 1, 1);
            var target = new SemanticVersion(1, 1, 1, 1);

            new GreaterRule(configured).Matches(target).ShouldBeFalse();
        }

        [Test]
        public void no_match_when_target_version_is_less_than_configured()
        {
            var configured = new SemanticVersion(1, 1, 0, 1);
            var target = new SemanticVersion(1, 0, 1, 1);

            new GreaterRule(configured).Matches(target).ShouldBeFalse();
        }
    }
}