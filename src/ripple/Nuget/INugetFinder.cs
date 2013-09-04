using ripple.Model;

namespace ripple.Nuget
{
    public interface INugetFinder
    {
        bool Matches(Dependency dependency);
        NugetResult Find(Solution solution, Dependency dependency);

        void Filter(Solution solution, Dependency dependency, NugetResult result);
    }
}