using System.Xml.Linq;
using FubuCore;
using NuGet;

namespace ripple.Nuget
{
    public class NuspecDependency
    {
        public NuspecDependency()
        {
        }

        public NuspecDependency(string name)
        {
            Name = name;
        }

        public NuspecDependency(string name, string version)
        {
            Name = name;

            if (version.IsNotEmpty())
            {
                VersionSpec = VersionUtility.ParseVersionSpec(version);
            }
        }

        public NuspecDependency(string name, IVersionSpec spec)
        {
            Name = name;
            VersionSpec = spec;
        }

        public string Name { get; set; }
        public IVersionSpec VersionSpec { get; set; }

        public bool MatchesName(string name)
        {
            return Name.EqualsIgnoreCase(name);
        }

        protected bool Equals(NuspecDependency other)
        {
            var versionsMatch = false;
            if (VersionSpec == null && other.VersionSpec == null)
            {
                versionsMatch = true;
            }
            else if (VersionSpec != null && other.VersionSpec != null)
            {
                versionsMatch = VersionSpec.ToString().Equals(other.VersionSpec.ToString());
            }

            return string.Equals(Name, other.Name) && versionsMatch;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NuspecDependency) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Name.GetHashCode();
            }
        }

		public override string ToString()
		{
			if (VersionSpec == null)
			{
				return "Name: " + Name;
			}

			return "Name: {0}; Version: {1}".ToFormat(Name, VersionSpec);
		}

        public static NuspecDependency ReadFrom(XElement element)
        {
            var version = element.GetOptionalAttributeValue("version");
            return new NuspecDependency(element.Attribute("id").Value, version);
        }
    }
}