using FubuCore;
using FubuCore.Descriptions;
using NuGet;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Packaging
{
    public class NuspecDependencyToken : DescribesItself
    {
        public NuspecDependencyToken(string dependency, string version, VersionConstraint constraint)
            : this(new Dependency(dependency), new SemanticVersion(version), constraint)
        {
        }

        public NuspecDependencyToken(string dependency, SemanticVersion version, VersionConstraint constraint)
            : this(new Dependency(dependency), version, constraint)
        {
        }

        public NuspecDependencyToken(Dependency dependency, SemanticVersion version, VersionConstraint constraint)
        {
            Constraint = constraint;
            Version = version;
            Dependency = dependency;
        }

        public Dependency Dependency { get; private set; }
        public SemanticVersion Version { get; private set; }
        public VersionConstraint Constraint { get; private set; }

        public NuspecDependency Create()
        {
            var version = GenerateVersion();
            return new NuspecDependency(Dependency.Name, version);
        }

        public IVersionSpec GenerateVersion()
        {
            return Constraint.SpecFor(Version);
        }

        public void Describe(Description description)
        {
            description.Title = Dependency.Name;
            description.ShortDescription = "Emitting as {0} from constraint: {1}".ToFormat(GenerateVersion(), Constraint);
        }

        protected bool Equals(NuspecDependencyToken other)
        {
            return Dependency.Equals(other.Dependency) && Version.Equals(other.Version) && Constraint.Equals(other.Constraint);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NuspecDependencyToken) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Dependency.GetHashCode();
                hashCode = (hashCode*397) ^ Version.GetHashCode();
                hashCode = (hashCode*397) ^ Constraint.GetHashCode();
                return hashCode;
            }
        }
    }
}