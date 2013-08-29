using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;
using ripple.Model.Versioning;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class DependencyVersionTester
    {
        [Test]
        public void matches_when_all_rules_match()
        {
            var r1 = new StubVersionRule(true);
            var r2 = new StubVersionRule(true);

            var version = new DependencyVersion();
            version.AddRule(r1);
            version.AddRule(r2);

            version.Matches(null).ShouldBeTrue();
        }

        [Test]
        public void no_match_when_any_rules_do_not_match()
        {
            var r1 = new StubVersionRule(true);
            var r2 = new StubVersionRule(false);

            var version = new DependencyVersion();
            version.AddRule(r1);
            version.AddRule(r2);

            version.Matches(null).ShouldBeFalse();
        }

        public class StubVersionRule : IVersionRule
        {
            private readonly bool _matches;

            public StubVersionRule(bool matches)
            {
                _matches = matches;
            }

            public bool Matches(SemanticVersion target)
            {
                return _matches;
            }
        }
    }
}