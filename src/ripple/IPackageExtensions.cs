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
            return package.DependencySets.SelectMany(set =>
            {
                return set.Dependencies.Select(x =>
                {
                    if (x.VersionSpec != null)
                    {
                        var version = x.VersionSpec.MaxVersion ?? x.VersionSpec.MinVersion;
                        return new Dependency(x.Id, version.ToString());
                    }

                    return new Dependency(x.Id);
                });
            }).Distinct();
         }
    }
}