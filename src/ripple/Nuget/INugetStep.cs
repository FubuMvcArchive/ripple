using System.Collections.Generic;

namespace ripple.Nuget
{
    public interface INugetStep
    {
        void Execute(INugetStepRunner runner);
        void AddSelf(IList<INugetStep> steps);
    }
}