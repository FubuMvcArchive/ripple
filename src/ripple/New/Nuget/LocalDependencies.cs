using System.Collections.Generic;
using System.Linq;
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
			return _dependencies.Single(x => x.Name == dependency.Name);
		}

		public bool Has(Dependency dependency)
		{
			return _dependencies.Any(x => x.Name == dependency.Name);
		}

		public IEnumerable<INugetFile> All()
		{
			return _dependencies;
		}
	}
}