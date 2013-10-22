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
    }
}