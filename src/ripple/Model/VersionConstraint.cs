using System;
using NuGet;

namespace ripple.Model
{
    public class VersionConstraint
    {
        public static readonly VersionConstraint DefaultFloat;
        public static readonly VersionConstraint DefaultFixed;

        static VersionConstraint()
        {
            DefaultFloat = new VersionConstraint(VersionToken.Current);
            DefaultFixed = new VersionConstraint(VersionToken.Current, VersionToken.NextMajor);
        }


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

        protected bool Equals(VersionConstraint other)
        {
            return Equals(_min, other._min) && Equals(_max, other._max);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((VersionConstraint) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_min != null ? _min.GetHashCode() : 0)*397) ^ (_max != null ? _max.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            var constraint = _min.Key;
            if (_max == null) return constraint;

            return constraint + "," + _max.Key;
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