using System;
using NuGet;

namespace ripple.Model
{
    public class VersionConstraint
    {
        private readonly VersionToken _min;
        private readonly VersionToken _max;

        public VersionConstraint(VersionToken min)
            : this(min, null)
        {
        }

        public VersionConstraint(VersionToken min, VersionToken max)
        {
            _min = min;
            _max = max;
        }

        public VersionToken Min
        {
            get { return _min; }
        }

        public VersionToken Max
        {
            get { return _max; }
        }

        public VersionSpec SpecFor(SemanticVersion version)
        {
            return new VersionSpec
            {
                IsMinInclusive = true,
                MinVersion = Min.Value(version),
                IsMaxInclusive = false,
                MaxVersion = Max == null ? null : Max.Value(version)
            };
        }

        public static VersionConstraint Parse(string value)
        {
            var tokens = value.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
            var min = VersionToken.Find(tokens[0]);
            if (tokens.Length == 1)
            {
                return new VersionConstraint(min);
            }

            var max = VersionToken.Find(tokens[1]);
            return new VersionConstraint(min, max);
        }
    }
}