using ripple.Model;

namespace ripple.Nuget
{
    public interface INugetFilter
    {
        void Filter(Solution solution, Dependency dependency, NugetResult result);
    }
}