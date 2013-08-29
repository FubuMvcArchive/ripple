using System.Diagnostics;
using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;
using ripple.Model.Versioning;

namespace ripple.Testing.Model
{
    [TestFixture]
    public class dependency_version_parsing
    {
        [Test]
        public void no_token_with_invalid_version_throws_exception()
        {
            Exception<InvalidSyntaxException>
                .ShouldBeThrownBy(() => parse("2.1.3+"));
        }

        [Test]
        public void invalid_token()
        {
            Debug.WriteLine(Exception<InvalidSyntaxException>.ShouldBeThrownBy(() => parse("2.1.3=")).Message);
        }

        [Test]
        public void specifying_a_version_with_no_token_defaults_to_equality()
        {
            parse("2.1");
            verifyRules(equality("2.1"));
        }

        [Test]
        public void explicit_equals_version()
        {
            parse("= 3.2");
            verifyRules(equality("3.2"));
        }

        [Test]
        public void not_equal_to_version()
        {
            parse("!= 2.6.4");
            verifyRules(inequality("2.6.4"));
        }

        [Test]
        public void greater_than_version()
        {
            parse("> 2.1");
            verifyRules(greaterThan("2.1"));
        }

        [Test]
        public void greater_than_or_equal_to_version()
        {
            parse(">= 2.0");
            verifyRules(greaterThanOrEqual("2.0"));
        }

        [Test]
        public void less_than_version()
        {
            parse("< 2.1");
            verifyRules(lessThan("2.1"));
        }

        [Test]
        public void less_than_or_equal_to_version()
        {
            parse("<= 2.0");
            verifyRules(lessThanOrEqual("2.0"));
        }

        [Test]
        public void range()
        {
            parse(">= 2.1, <= 2.3");
            verifyRules(greaterThanOrEqual("2.1"), lessThanOrEqual("2.3"));
        }

        [Test]
        public void approximate_version()
        {
            parse("~> 3.0");
            verifyRules(approximatelyGreater("3.0"));
        }

        [Test]
        public void approximate_with_less_than()
        {
            parse("~>1.1, <2.0");
            verifyRules(approximatelyGreater("1.1"), lessThan("2.0"));
        }

        private DependencyVersion theVersion;

        private void parse(string text)
        {
            theVersion = DependencyVersion.Parse(text);
        }

        private void verifyRules(params IVersionRule[] rules)
        {
            theVersion.Rules.ShouldHaveTheSameElementsAs(rules);
        }

        private IVersionRule equality(string version)
        {
            return new EqualityRule(SemanticVersion.Parse(version));
        }

        private IVersionRule inequality(string version)
        {
            return new InequalityRule(SemanticVersion.Parse(version));
        }

        private IVersionRule greaterThan(string version)
        {
            return new GreaterRule(SemanticVersion.Parse(version));
        }

        private IVersionRule lessThan(string version)
        {
            return new LessThanRule(SemanticVersion.Parse(version));
        }

        private IVersionRule greaterThanOrEqual(string version)
        {
            return new GreaterThanOrEqualRule(SemanticVersion.Parse(version));
        }

        private IVersionRule lessThanOrEqual(string version)
        {
            return new LessThanOrEqualRule(SemanticVersion.Parse(version));
        }

        private IVersionRule approximatelyGreater(string version)
        {
            return new ApproximatelyGreaterRule(SemanticVersion.Parse(version));
        }
    }
}