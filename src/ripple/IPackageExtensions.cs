using System.Collections.Generic;
using System.Linq;
using NuGet;
using ripple.Model;

namespace ripple
{
    public static class PackageExtensions
    {
        public static IEnumerable<Dependency> ImmediateDependencies(this IPackage package)
        {
            if (package == null)
            {
                return new Dependency[0];
            }

            return package.DependencySets.SelectMany(set =>
            {
                return set.Dependencies.Select(x =>
                {
                    if (x.VersionSpec != null)
                    {
                        return new Dependency(x.Id, x.VersionSpec);
                    }

                    return new Dependency(x.Id);
                });
            }).Distinct();
        }
    }
}