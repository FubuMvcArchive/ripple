using FubuObjectBlocks;

namespace ripple.Model
{
    public class NuspecSettings
    {
        public NuspecSettings()
        {
            Float = VersionConstraint.DefaultFloat;
            Fixed = VersionConstraint.DefaultFixed;
        }

        [ImplicitValue]
        public VersionConstraint Float { get; set; }
        [ImplicitValue]
        public VersionConstraint Fixed { get; set; } 

        public VersionConstraint ConstraintFor(UpdateMode mode)
        {
            return mode == UpdateMode.Fixed ? Fixed : Float;
        }

        protected bool Equals(NuspecSettings other)
        {
            return Float.Equals(other.Float) && Fixed.Equals(other.Fixed);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NuspecSettings) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Float.GetHashCode()*397) ^ Fixed.GetHashCode();
            }
        }
    }
}