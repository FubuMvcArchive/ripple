using ripple.Model;

namespace ripple.Nuget
{
    public class DefaultFinder : INugetFinder
    {
        public bool Matches(Dependency dependency)
        {
            return true;
        }

        public NugetResult Find(Solution solution, Dependency dependency)
        {
            return NugetSearch.FindNuget(solution, dependency);
        }
    }
}