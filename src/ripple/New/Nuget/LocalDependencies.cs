using System.Collections.Generic;
using System.Linq;
using FubuCore;
using ripple.New.Model;

namespace ripple.New.Nuget
{
	public class LocalDependencies
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
			return _dependencies.Single(x => x.Name.EqualsIgnoreCase(name));
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
	}
}