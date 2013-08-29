using NuGet;

namespace ripple.Model.Versioning
{
    public class EqualityRule : IVersionRule
    {
        private readonly SemanticVersion _configured;

        public EqualityRule(SemanticVersion configured)
        {
            _configured = configured;
        }

        public bool Matches(SemanticVersion target)
        {
            return _configured.Equals(target);
        }

        protected bool Equals(EqualityRule other)
        {
            return _configured.Equals(other._configured);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EqualityRule)obj);
        }

        public override int GetHashCode()
        {
            return _configured.GetHashCode();
        }
    }
}