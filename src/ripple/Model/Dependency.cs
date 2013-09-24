using System;
using System.Xml;
using System.Xml.Serialization;
using FubuCore;
using FubuCore.Descriptions;
using NuGet;
using ripple.Nuget;

namespace ripple.Model
{
    public class Dependency : DescribesItself
    {
        private IVersionSpec _versionSpec;

        public Dependency()
        {
        }

        public Dependency(string name)
            : this(name, string.Empty)
        {
        }

        public Dependency(string name, string version)
            : this(name, version, UpdateMode.Float)
        {
        }

        public Dependency(string name, UpdateMode mode)
            : this(name, string.Empty, mode)
        {
        }

        public Dependency(string name, string version, UpdateMode mode)
        {
            Name = name;
            Version = version ?? string.Empty;

            Mode = mode;
        }

        public Dependency(string name, SemanticVersion version, UpdateMode mode)
            : this(name, version.ToString(), mode)
        {
        }

        public Dependency(string name, IVersionSpec versionSpec)
            : this(name)
        {
            if (versionSpec.MinVersion != null && versionSpec.MaxVersion != null &&
                versionSpec.MinVersion == versionSpec.MaxVersion)
            {
                Mode = UpdateMode.Fixed;
                Version = versionSpec.MinVersion.Version.ToString();
            }
            else
            {
                Mode = UpdateMode.Float;
            }

            _versionSpec = versionSpec;
        }

        public IVersionSpec VersionSpec
        {
            get { return _versionSpec; }
        }


        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Version { get; set; }
        [XmlAttribute]
        public UpdateMode Mode { get; set; }
        [XmlAttribute]
        public string Stability
        {
            get { return NugetStability.HasValue ? NugetStability.Value.ToString() : null; }
            set
            {
                if (value.IsNotEmpty())
                {
                    NugetStability = (NugetStability)Enum.Parse(typeof(NugetStability), value);
                }
            }
        }

        [XmlAttribute]
        public string Constraint
        {
            get { return VersionConstraint == null ? null : VersionConstraint.ToString(); }
            set
            {
                if (value.IsNotEmpty())
                {
                    VersionConstraint = VersionConstraint.Parse(value);
                }
            }
        }

        [XmlIgnore]
        public VersionConstraint VersionConstraint { get; set; }

        [XmlIgnore]
        public NugetStability? NugetStability { get; set; }

        public NugetStability DetermineStability(NugetStability stability)
        {
            return NugetStability ?? stability;
        }


        public SemanticVersion SemanticVersion()
        {
            return Version.IsNotEmpty() ? NuGet.SemanticVersion.Parse(Version) : null;
        }

        public void FixAt(string version)
        {
            Version = version;
            Mode = UpdateMode.Fixed;
        }

        public bool IsFloat()
        {
            return Mode == UpdateMode.Float;
        }

        public bool IsFixed()
        {
            return Mode == UpdateMode.Fixed;
        }

        public bool Equals(Dependency other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name) && Equals(other.SemanticVersion(), SemanticVersion());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Dependency)) return false;
            return Equals((Dependency)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Version != null ? Version.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            var value = Name;
            if (Version.IsNotEmpty())
            {
                value += "," + Version;
            }

            if (!IsFloat())
            {
                value += "," + Mode.ToString();
            }

            return value;
        }

        public void Describe(Description description)
        {
            description.Title = Name;
            description.ShortDescription = IsFloat() ? UpdateMode.Float.ToString() : Version;
        }

        public void Float()
        {
            Version = string.Empty;
            Mode = UpdateMode.Float;
        }

        public Dependency AsFloat()
        {
            var floated = Copy();
            floated.Float();

            return floated;
        }

        public Dependency AsFixed()
        {
            var @fixed = Copy();
            @fixed.Mode = UpdateMode.Fixed;

            return @fixed;
        }

        public Dependency Copy()
        {
            return MemberwiseClone().As<Dependency>();
        }

        public bool MatchesName(Dependency dependency)
        {
            return MatchesName(dependency.Name);
        }

        public bool MatchesName(string name)
        {
            return Name.EqualsIgnoreCase(name);
        }

        public static Dependency For(INugetFile nuget)
        {
            return new Dependency(nuget.Name, nuget.Version.ToString());
        }

        public static Dependency FloatFor(string name)
        {
            return new Dependency(name, UpdateMode.Float);
        }

        public static Dependency Parse(string input)
        {
            var parts = input.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                throw new ArgumentOutOfRangeException("input", "Could not parse Dependency: " + input);
            }

            var dependency = new Dependency(parts[0]);
            if (parts.Length > 1)
            {
                dependency.Version = parts[1];
            }

            if (parts.Length > 2)
            {
                dependency.Mode = (UpdateMode)Enum.Parse(typeof(UpdateMode), parts[2]);
            }

            return dependency;
        }

        public static Dependency ReadFrom(XmlElement element)
        {
            return new Dependency(element.GetAttribute("id"), element.GetAttribute("version"), UpdateMode.Fixed);
        }

        public bool IsReleasedOnly()
        {
            return DetermineStability(Nuget.NugetStability.ReleasedOnly) == Nuget.NugetStability.ReleasedOnly;
        }

        public bool IncludesPrelease()
        {
            return !IsReleasedOnly();
        }

        public bool MatchesVersionSpec(Predicate<IVersionSpec> predicate)
        {
            if (VersionSpec == null) return false;
            return predicate(VersionSpec);
        }
    }
}