using System;
using System.Linq;
using NuGet;

namespace ripple.Model.Versioning
{
    public class ApproximatelyGreaterRule : IVersionRule
    {
        private readonly SemanticVersion _configured;

        public ApproximatelyGreaterRule(SemanticVersion configured)
        {
            _configured = configured;
        }

        public bool Matches(SemanticVersion target)
        {
            var granularity = determineGranularity(_configured);
            var next = nextVersion(_configured.Version, granularity);

            return target >= _configured && target < next;
        }

        protected bool Equals(ApproximatelyGreaterRule other)
        {
            return _configured.Equals(other._configured);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ApproximatelyGreaterRule) obj);
        }

        public override int GetHashCode()
        {
            return _configured.GetHashCode();
        }

        private static SemanticVersion nextVersion(Version version, VersionGranularity granularity)
        {
            switch (granularity)
            {
                case VersionGranularity.Major:
                    return new SemanticVersion(version.Major + 1, 0, 0, 0);
                case VersionGranularity.Minor:
                    return new SemanticVersion(version.Major, version.Minor + 1, 0, 0);
                case VersionGranularity.Patch:
                    return new SemanticVersion(version.Major, version.Minor, version.Build + 1, 0);
            }

            return null;
        }

        private static VersionGranularity determineGranularity(SemanticVersion version)
        {
            var count = version.ToString().Count(x => x == '.');
            switch (count)
            {
                case 1: // 2.0
                    return VersionGranularity.Major;
                case 2: // 2.0.0
                    return VersionGranularity.Minor;
                default: // 2.0.0.0
                    return VersionGranularity.Patch;
            }
        }

        public enum VersionGranularity
        {
            Major,
            Minor,
            Patch
        }
    }
}