namespace ripple.Model
{
    public class NuspecSettings
    {
        public NuspecSettings()
        {
            Float = VersionConstraint.DefaultFloat;
            Fixed = VersionConstraint.DefaultFixed;
        }

        public VersionConstraint Float { get; set; }
        public VersionConstraint Fixed { get; set; } 

        public VersionConstraint ConstraintFor(UpdateMode mode)
        {
            return mode == UpdateMode.Fixed ? Fixed : Float;
        }
    }
}