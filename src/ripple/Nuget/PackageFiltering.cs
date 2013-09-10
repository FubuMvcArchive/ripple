using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace ripple.Nuget
{
    public static class PackageFiltering
    {
        public static IPackage Latest(this IEnumerable<IPackage> values)
        {
            var candidates = values.ToArray();
            var candidate = candidates.FirstOrDefault(x => x.IsAbsoluteLatestVersion)
                            ?? candidates.FirstOrDefault(x => x.IsLatestVersion);

            if (candidate == null)
            {
                // If both absolute and latest are false, then we order in descending order (by version) and take the top
                candidate = candidates
                    .OrderByDescending(x => x.Version)
                    .FirstOrDefault();
            }

            return candidate;
        }

        public static IRemoteNuget LatestNuget(this IEnumerable<IPackage> candidates)
        {
            var candidate = candidates.Latest();
            return candidate == null ? null : new RemoteNuget(candidate);
        }
    }
}