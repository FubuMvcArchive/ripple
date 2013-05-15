using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class VersionConstraintTester
    {
        [Test]
        public void parse_only_a_min_value()
        {
            var constraint = VersionConstraint.Parse("Current");
            constraint.Min.ShouldEqual(VersionToken.Current);
            constraint.Max.ShouldBeNull();
        }

        [Test]
        public void parse_min_and_max()
        {
            var constraint = VersionConstraint.Parse("Current,NextMin");
            constraint.Min.ShouldEqual(VersionToken.Current);
            constraint.Max.ShouldEqual(VersionToken.NextMin);
        }

        [Test]
        public void spec_for_min()
        {
            var version = new SemanticVersion("1.1.0.0");
            var constraint = VersionConstraint.Parse("Current");
            var spec = constraint.SpecFor(version);

            spec.MinVersion.ShouldEqual(version);
            spec.IsMinInclusive.ShouldBeTrue();

            spec.MaxVersion.ShouldBeNull();
        }

        [Test]
        public void spec_for_min_and_max()
        {
            var version = new SemanticVersion("1.1.0.0");
            var constraint = VersionConstraint.Parse("Current,NextMajor");
            var spec = constraint.SpecFor(version);

            spec.MinVersion.ShouldEqual(version);
            spec.IsMinInclusive.ShouldBeTrue();

            spec.MaxVersion.ShouldEqual(new SemanticVersion("2.0.0.0"));
            spec.IsMaxInclusive.ShouldBeFalse();
        }
    }
}