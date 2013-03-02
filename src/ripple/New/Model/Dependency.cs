using FubuCore;
using FubuCore.Descriptions;

namespace ripple.New.Model
{
	public class Dependency : DescribesItself
	{
		private readonly string _name;
		private readonly string _version;

		public Dependency(string name)
		{
			_name = name;
			_version = string.Empty;
		}

		public Dependency(string name, string version)
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

		public bool Equals(Dependency other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other._name, _name) && Equals(other._version, _version);
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