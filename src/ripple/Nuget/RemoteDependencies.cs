using System.Collections.Generic;
using System.Linq;

namespace ripple.Nuget
{
	public class RemoteDependencies
	{
		private readonly IEnumerable<IRemoteNuget> _dependencies;

		public RemoteDependencies(IEnumerable<IRemoteNuget> dependencies)
		{
			_dependencies = dependencies;
		}

		public bool Any()
		{
			return _dependencies.Any();
		}

		public IEnumerable<IRemoteNuget> All()
		{
			return _dependencies;
		}
	}
}