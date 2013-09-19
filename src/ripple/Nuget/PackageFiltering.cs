using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace ripple.Nuget
{
    public static class PackageFiltering
    {
        public static IPackage Latest(this IEnumerable<IPackage> values, IVersionSpec spec)
        {
            var candidates = values.ToArray();
            var candidate = candidates.FirstOrDefault(x => x.IsAbsoluteLatestVersion)
                            ?? candidates.FirstOrDefault(x => x.IsLatestVersion);

            if (candidate == null)
            {
                // If both absolute and latest are false, then we order in descending order (by version) and take the top
                var ordered = candidates
                    .OrderByDescending(x => x.Version)
                    .ToArray();

                if (spec == null)
                {
                    candidate = ordered.FirstOrDefault();
                }
                else
                {
                    Func<IPackage, bool> predicate;
                    if (spec.IsMaxInclusive)
                    {
                        predicate = x => x.Version <= spec.MaxVersion;
                    }
                    else
                    {
                        predicate = x => x.Version < spec.MaxVersion;
                    }

                    candidate = ordered.FirstOrDefault(predicate);
                }
            }

            return candidate;
        }

        public static IRemoteNuget LatestNuget(this IEnumerable<IPackage> candidates, IVersionSpec spec = null)
        {
            var candidate = candidates.Latest(spec);
            return candidate == null ? null : new RemoteNuget(candidate);
        }
    }
}