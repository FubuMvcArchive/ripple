using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class integrated_dependency_version_parsing
    {
        [Test]
        public void parse_and_match_an_approximate_version()
        {
            var version = new DependencyVersion("~>2.0,<=2.5");

            version.Matches(SemanticVersion.Parse("2.0")).ShouldBeTrue();
            version.Matches(SemanticVersion.Parse("2.1")).ShouldBeTrue();
            version.Matches(SemanticVersion.Parse("2.2")).ShouldBeTrue();
            version.Matches(SemanticVersion.Parse("2.3")).ShouldBeTrue();
            version.Matches(SemanticVersion.Parse("2.4")).ShouldBeTrue();
            version.Matches(SemanticVersion.Parse("2.5")).ShouldBeTrue();
            version.Matches(SemanticVersion.Parse("2.6")).ShouldBeFalse();
        }
    }
}