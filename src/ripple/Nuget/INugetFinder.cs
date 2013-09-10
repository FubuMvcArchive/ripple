using ripple.Model;

namespace ripple.Nuget
{
    public interface INugetFinder
    {
        bool Matches(Dependency dependency);
        NugetResult Find(Solution solution, Dependency dependency);
    }
}