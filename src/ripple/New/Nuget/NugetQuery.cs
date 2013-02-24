using FubuCore;
using FubuCore.Descriptions;
using ripple.New.Model;

namespace ripple.New.Nuget
{
	public class NugetQuery : DescribesItself
	{
		public NugetQuery()
		{
			Stability = NugetStability.Anything;
		}

		public string Name { get; set; }
		public string Version { get; set; }
		public NugetStability Stability { get; set; }

		public bool IsFloat()
		{
			return Version.IsEmpty();
		}

		public override string ToString()
		{
			return string.Format("Name: {0}, Version: {1}, Stability: {2}", Name, Version, Stability);
		}

		public void Describe(Description description)
		{
			description.Title = Name;
			description.ShortDescription = IsFloat() ? UpdateMode.Float.ToString() : Version;
		}

		protected bool Equals(NugetQuery other)
		{
			return string.Equals(Name, other.Name) && string.Equals(Version, other.Version);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((NugetQuery)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Name.GetHashCode() * 397) ^ Version.GetHashCode();
			}
		}

		public static NugetQuery For(NugetDependency dependency)
		{
			return new NugetQuery
			{
				Name = dependency.Name,
				Version = dependency.Version
			};
		}
	}
}