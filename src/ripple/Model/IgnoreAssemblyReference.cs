using FubuCore;

namespace ripple.Model
{
    public class IgnoreAssemblyReference
    {
        public string Package { get; set; }
        public string Assembly { get; set; }

        public bool Matches(Dependency dependency, string assembly)
        {
            return dependency.MatchesName(Package) && Assembly.Replace(".dll", "").EqualsIgnoreCase(assembly);
        }

        protected bool Equals(IgnoreAssemblyReference other)
        {
            return string.Equals(Package, other.Package) && string.Equals(Assembly, other.Assembly);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IgnoreAssemblyReference) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Package.GetHashCode()*397) ^ Assembly.GetHashCode();
            }
        }
    }
}