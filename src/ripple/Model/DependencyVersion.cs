using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore.Util;
using NuGet;
using ripple.Model.Versioning;

namespace ripple.Model
{
    public class DependencyVersion
    {
        public static readonly Cache<string, Func<SemanticVersion, IVersionRule>> VersionRules;
        private static readonly IEnumerable<string> Tokens; 

        static DependencyVersion()
        {
            VersionRules = new Cache<string, Func<SemanticVersion, IVersionRule>>(token =>
            {
                throw new InvalidOperationException("Unrecognized token: " + token);
            });

            VersionRules.Fill("=", version => new EqualityRule(version));
            VersionRules.Fill("!=", version => new InequalityRule(version));
            VersionRules.Fill(">", version => new GreaterRule(version));
            VersionRules.Fill("<", version => new LessThanRule(version));
            VersionRules.Fill(">=", version => new GreaterThanOrEqualRule(version));
            VersionRules.Fill("<=", version => new LessThanOrEqualRule(version));
            VersionRules.Fill("~>", version => new ApproximatelyGreaterRule(version));

            Tokens = VersionRules.GetAllKeys();
        }

        private readonly IList<IVersionRule> _rules = new List<IVersionRule>();

        public DependencyVersion()
        {
        }

        public DependencyVersion(string input)
            : this(Parse(input))
        {
        }

        private DependencyVersion(DependencyVersion inner)
        {
            _rules = inner._rules;
        }

        public IEnumerable<IVersionRule> Rules { get { return _rules; } }

        public void AddRule(IVersionRule rule)
        {
            _rules.Add(rule);
        }

        public bool Matches(SemanticVersion version)
        {
            return _rules.All(x => x.Matches(version));
        }

        public static DependencyVersion Parse(string input)
        {
            if (!Tokens.Any(input.Contains))
            {
                return defaultToken(input);
            }
            if (!Tokens.Any(input.StartsWith))
            {
                throw new InvalidSyntaxException("Invalid token. Expected one of the following: " + Tokens.Select(x => "'" + x + "'").Join(", "));
            }

            var version = new DependencyVersion();
            var tokenizer = new VersionTokenizer();

            version._rules.AddRange(tokenizer.Tokenize(input));

            return version;
        }

        private static DependencyVersion defaultToken(string input)
        {
            SemanticVersion semanticVersion;
            if (!SemanticVersion.TryParse(input, out semanticVersion))
            {
                throw new InvalidSyntaxException("Invalid version: " + input);
            }

            var version = new DependencyVersion();
            version.AddRule(new EqualityRule(semanticVersion));
            return version;
        }
    }
}