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
			Stability = NugetStability.Anything;
		}

        public Dependency(string name, SemanticVersion version, UpdateMode mode)
            : this(name, version.ToString(), mode)
        {
        }

		[XmlAttribute]
		public string Name { get; set; }
		[XmlAttribute]
		public string Version { get; set; }
		[XmlAttribute]
		public UpdateMode Mode { get; set; }
		[XmlAttribute]
		public NugetStability Stability { get; set; }

		public SemanticVersion SemanticVersion()
		{
			return NuGet.SemanticVersion.Parse(Version);
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
			return Equals(other.Name, Name) && Equals(other.Version, Version);
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
			var parts = input.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
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
				dependency.Mode = (UpdateMode) Enum.Parse(typeof (UpdateMode), parts[2]);
			}

			return dependency;
		}

		public static Dependency ReadFrom(XmlElement element)
		{
			return new Dependency(element.GetAttribute("id"), element.GetAttribute("version"), UpdateMode.Fixed);
		}
	}
}