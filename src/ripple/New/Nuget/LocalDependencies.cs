using System.Collections.Generic;
using System.Linq;

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

		public IEnumerable<INugetFile> All()
		{
			return _dependencies;
		}

		public bool ShouldUpdate(IRemoteNuget nuget)
		{
			var dependency = _dependencies.SingleOrDefault(x => x.Name == nuget.Name);
			if (dependency == null)
			{
				return false;
			}

			return nuget.Version > dependency.Version;
		}
	}
}