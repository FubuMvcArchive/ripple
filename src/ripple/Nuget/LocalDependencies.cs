using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Logging;
using ripple.Model;

namespace ripple.Nuget
{
	public class LocalDependencies : DescribesItself, LogTopic
	{
		private readonly IEnumerable<INugetFile> _dependencies;

		public LocalDependencies(IEnumerable<INugetFile> dependencies)
		{
			_dependencies = dependencies;
		}

		public bool Any()
		{
			return _dependencies.Any();
		}

		public INugetFile Get(Dependency dependency)
		{
			return Get(dependency.Name);
		}

		public INugetFile Get(string name)
		{
			var nuget = _dependencies.SingleOrDefault(x => x.Name.EqualsIgnoreCase(name));
			if (nuget == null)
			{
				RippleLog.DebugMessage(this);
				throw new ArgumentOutOfRangeException("name", "Could not find " + name);
			}

			return nuget;
		}

		public bool Has(Dependency dependency)
		{
			return Has(dependency.Name);
		}

		public bool Has(string name)
		{
			return _dependencies.Any(x => x.Name == name);
		}

		public IEnumerable<INugetFile> All()
		{
			return _dependencies;
		}

		public void Describe(Description description)
		{
			description.AddList("Items", _dependencies);
		}
	}
}