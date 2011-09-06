namespace ripple
{
    public class MigrationPath
    {
        public NugetSpec Nuget { get; set; }
        public Project Project { get; set; }

        public bool Equals(MigrationPath other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Nuget, Nuget) && Equals(other.Project, Project);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (MigrationPath)) return false;
            return Equals((MigrationPath) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Nuget != null ? Nuget.GetHashCode() : 0)*397) ^ (Project != null ? Project.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("Nuget: {0} to Project: {1}", Nuget, Project);
        }
    }
}