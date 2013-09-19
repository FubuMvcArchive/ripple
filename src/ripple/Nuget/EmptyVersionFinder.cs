using FubuCore;
using ripple.Model;

namespace ripple.Nuget
{
    public class EmptyVersionFinder : INugetFinder
    {
        public bool Matches(Dependency dependency)
        {
            return dependency.Version.IsEmpty();
        }

        public NugetResult Find(Solution solution, Dependency dependency)
        {
            return NugetSearch.FindLatestByName(solution, dependency.Name);
        }
    }
}