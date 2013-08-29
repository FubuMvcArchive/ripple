using NuGet;

namespace ripple.Model.Versioning
{
    public class LessThanOrEqualRule : IVersionRule
    {
        private readonly SemanticVersion _configured;

        public LessThanOrEqualRule(SemanticVersion configured)
        {
            _configured = configured;
        }

        public bool Matches(SemanticVersion target)
        {
            return target <= _configured;
        }

        protected bool Equals(LessThanOrEqualRule other)
        {
            return _configured.Equals(other._configured);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LessThanOrEqualRule) obj);
        }

        public override int GetHashCode()
        {
            return _configured.GetHashCode();
        }
    }
}