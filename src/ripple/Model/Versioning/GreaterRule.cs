using NuGet;

namespace ripple.Model.Versioning
{
    public class GreaterRule : IVersionRule
    {
        private readonly SemanticVersion _configured;

        public GreaterRule(SemanticVersion configured)
        {
            _configured = configured;
        }

        public bool Matches(SemanticVersion target)
        {
            return target > _configured;
        }

        protected bool Equals(GreaterRule other)
        {
            return _configured.Equals(other._configured);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((GreaterRule) obj);
        }

        public override int GetHashCode()
        {
            return _configured.GetHashCode();
        }
    }
}