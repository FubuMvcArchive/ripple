using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace ripple.Packaging
{
    public class DirectDependenciesSource : INuspecDependencySource
    {
        public IEnumerable<NuspecDependencyToken> DetermineDependencies(NuspecTemplateContext context)
        {
            var local = context.Solution.LocalDependencies();
            return context
                .Current
                .DetermineDependencies()
                .Select(dependency =>
                {
                    var constraint = context.Solution.ConstraintFor(dependency);
                    SemanticVersion version;
                    if (local.Has(dependency))
                    {
                        version = local.Get(dependency).Version;
                    }
                    else if(dependency.Version.IsEmpty())
                    {
                        throw new InvalidOperationException(dependency.Name + " is not configured with a version and no local copy exists");
                    }
                    else
                    {
                        version = dependency.SemanticVersion();
                    }
                    

                    return new NuspecDependencyToken(dependency, version, constraint);
                });
        }
    }
}