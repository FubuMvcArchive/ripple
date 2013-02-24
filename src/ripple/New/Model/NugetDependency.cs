using FubuCore;
using FubuCore.Descriptions;

namespace ripple.New.Model
{
	public class NugetDependency : DescribesItself
	{
		private readonly string _name;
		private readonly string _version;

		public NugetDependency(string name)
		{
			_name = name;
			_version = string.Empty;
		}

		public NugetDependency(string name, string version)
		{
			_name = name;
			_version = version ?? string.Empty;
		}

		public string Name
		{
			get { return _name; }
		}

		public string Version
		{
			get { return _version; }
		}

		public bool Equals(NugetDependency other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other._name, _name) && Equals(other._version, _version);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(NugetDependency)) return false;
			return Equals((NugetDependency)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((_name != null ? _name.GetHashCode() : 0) * 397) ^ (_version != null ? _version.GetHashCode() : 0);
			}
		}

		public override string ToString()
		{
			return string.Format("{0} -- {1}", _name, _version);
		}

		public void Describe(Description description)
		{
			description.Title = Name;
			description.ShortDescription = Version.IsEmpty() ? UpdateMode.Float.ToString() : Version;
		}
	}
}