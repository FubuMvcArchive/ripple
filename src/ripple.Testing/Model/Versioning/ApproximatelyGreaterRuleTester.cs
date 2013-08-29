using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model.Versioning;

namespace ripple.Testing.Model.Versioning
{
    [TestFixture]
    public class ApproximatelyGreaterRuleTester
    {
        [Test]
        public void matches_lower_bounds_of_major_version()
        {
            matches("2.0", "2.0").ShouldBeTrue();
        }

        [Test]
        public void matches_lower_bounds_of_major_and_minor()
        {
            matches("2.0", "2.1").ShouldBeTrue();
            matches("2.0", "2.9").ShouldBeTrue();
        }

        [Test]
        public void no_match_on_upper_bounds_of_major()
        {
            matches("2.0", "3.0").ShouldBeFalse();
        }

        [Test]
        public void matches_lower_bounds_of_major_and_minor_version()
        {
            matches("2.1.0", "2.1.0").ShouldBeTrue();
        }

        [Test]
        public void matches_lower_bounds_of_major_minor_and_patch_version()
        {
            matches("2.1.0", "2.1.9").ShouldBeTrue();
        }


        [Test]
        public void no_match_for_upper_bounds_of_major_and_minor_Version()
        {
            matches("2.1.0", "2.2.0").ShouldBeFalse();
        }

        [Test]
        public void matches_lower_bounds_of_major_minor_patch_version()
        {
            matches("2.1.0.0", "2.1.0.0").ShouldBeTrue();
        }

        [Test]
        public void matches_lower_bounds_of_major_minor_patch_and_build_version()
        {
            matches("2.1.0.0", "2.1.0.9").ShouldBeTrue();
        }


        [Test]
        public void no_match_for_upper_bounds_of_major_minor_and_patch_Version()
        {
            matches("2.1.0.0", "2.1.1.0").ShouldBeFalse();
        }
        

        private bool matches(string configured, string target)
        {
            return new ApproximatelyGreaterRule(SemanticVersion.Parse(configured)).Matches(SemanticVersion.Parse(target));
        }
    }
}