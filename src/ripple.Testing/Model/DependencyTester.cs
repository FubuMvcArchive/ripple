using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class DependencyTester
    {
        [Test]
        public void is_float()
        {
            new Dependency("Bottles", "1.0.0.0").IsFloat().ShouldBeTrue();
        }

        [Test]
        public void not_float()
        {
            new Dependency("Bottles", "1.0.0.0", UpdateMode.Fixed).IsFloat().ShouldBeFalse();
        }

        [Test]
        public void not_fixed()
        {
            new Dependency("Bottles", "1.0.0.0").IsFixed().ShouldBeFalse();
        }

        [Test]
        public void is_fixed()
        {
            new Dependency("Bottles", "1.0.0.0", UpdateMode.Fixed).IsFixed().ShouldBeTrue();
        }

        [Test]
        public void make_float()
        {
            var dependency = new Dependency("StructureMap", "2.6.3", UpdateMode.Fixed);
            dependency.Float();

            dependency.IsFloat().ShouldBeTrue();
        }

        [Test]
        public void to_string()
        {
            new Dependency("Bottles").ToString().ShouldEqual("Bottles");
            new Dependency("Bottles", "1.0.0.0").ToString().ShouldEqual("Bottles,1.0.0.0");
            new Dependency("Bottles", "1.0.0.1", UpdateMode.Fixed).ToString().ShouldEqual("Bottles,1.0.0.1,Fixed");
        }

        [Test]
        public void determine_stability()
        {
            new Dependency("FubuCore").DetermineStability(NugetStability.Anything).ShouldEqual(NugetStability.Anything);
            new Dependency("FubuCore") { NugetStability = NugetStability.ReleasedOnly }.DetermineStability(NugetStability.Anything).ShouldEqual(NugetStability.ReleasedOnly);
        }

        [Test]
        public void parsing_stability()
        {
            new Dependency("FubuCore") { Stability = "Anything" }.NugetStability.ShouldEqual(NugetStability.Anything);
            new Dependency("FubuCore") { Stability = "ReleasedOnly" }.NugetStability.ShouldEqual(NugetStability.ReleasedOnly);
        }

        [Test]
        public void released_only()
        {
            new Dependency("FubuCore").IsReleasedOnly().ShouldBeTrue();
            new Dependency("FubuCore") { NugetStability = NugetStability.ReleasedOnly }.IsReleasedOnly().ShouldBeTrue();
        }

        [Test]
        public void released_only_negative()
        {
            new Dependency("FubuCore") { NugetStability = NugetStability.Anything }.IsReleasedOnly().ShouldBeFalse();
        }

        [Test]
        public void gets_the_constraint_attribute()
        {
            var dependency = new Dependency("FubuCore") { VersionConstraint = VersionConstraint.DefaultFloat };

            dependency.Constraint.ShouldEqual(VersionConstraint.DefaultFloat.ToString());
        }

        [Test]
        public void gets_the_constraint_attribute_for_no_constraint()
        {
            var dependency = new Dependency("FubuCore");

            dependency.Constraint.ShouldBeNull();
        }

        [Test]
        public void parses_the_constraint_attribute()
        {
            var dependency = new Dependency("FubuCore") { Constraint = VersionConstraint.DefaultFloat.ToString() };
            dependency.VersionConstraint.ShouldEqual(VersionConstraint.DefaultFloat);
        }

        [Test]
        public void equals_uses_semantic_version()
        {
            var dep1 = new Dependency("structuremap", "2.6.3");
            var dep2 = new Dependency("structuremap", "2.6.3.0");

            dep1.ShouldEqual(dep2);
            dep2.ShouldEqual(dep1);
        }

        [Test]
        public void matches_version_spec_when_spec_is_null_just_returns_false()
        {
            var dep = new Dependency("Test");
            dep.MatchesVersionSpec(x => x.MinVersion != null).ShouldBeFalse();
        }
    }
}