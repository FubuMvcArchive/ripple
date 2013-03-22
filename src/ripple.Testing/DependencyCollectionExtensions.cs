using System.Collections.Generic;
using FubuTestingSupport;
using ripple.Model;

namespace ripple.Testing
{
	public static class DependencyCollectionExtensions
	{
		 public static void ShouldHaveTheSameDependenciesAs(this IEnumerable<Dependency> collection, params string[] names)
		 {
			 collection.ShouldHaveTheSameElementKeysAs(names, x => x.Name);
		 }
	}
}