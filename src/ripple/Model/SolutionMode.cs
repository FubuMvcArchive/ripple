namespace ripple.Model
{
    public class SolutionMode
    {
        public static readonly SolutionMode Ripple = new SolutionMode("Ripple");
        public static readonly SolutionMode NuGet = new SolutionMode("NuGet");

        private readonly string _value;

        private SolutionMode(string value)
        {
            _value = value;
        }

        protected bool Equals(SolutionMode other)
        {
            return string.Equals(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SolutionMode) obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(SolutionMode left, SolutionMode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SolutionMode left, SolutionMode right)
        {
            return !Equals(left, right);
        }
    }
}