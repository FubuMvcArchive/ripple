using System.Collections.Generic;

namespace ripple.Packaging
{
    public interface INuspecDependencySource
    {
        IEnumerable<NuspecDependencyToken> DetermineDependencies(NuspecTemplateContext context);
    }
}