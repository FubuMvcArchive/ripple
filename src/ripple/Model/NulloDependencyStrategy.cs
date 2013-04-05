using System.Collections.Generic;
using ripple.Nuget;

namespace ripple.Model
{
	public class NulloDependencyStrategy : IDependencyStrategy
	{
		public bool Matches(Project project)
		{
			return true;
		}

		public IEnumerable<Dependency> Read(Project project)
		{
			yield break;
		}

		public INugetFile FileFor(string path)
		{
			throw new System.NotImplementedException();
		}

		public void Write(Project project)
		{
			// no-op
		}

		public void RemoveDependencyConfigurations(Project project)
		{
			// no-op
		}
	}
}