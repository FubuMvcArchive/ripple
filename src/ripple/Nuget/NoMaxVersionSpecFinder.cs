using ripple.Model;

namespace ripple.Nuget
{
    public class NoMaxVersionSpecFinder : INugetFinder
    {
        public bool Matches(Dependency dependency)
        {
            return dependency.MatchesVersionSpec(spec => spec.MinVersion != null && spec.MaxVersion == null);
        }

        public NugetResult Find(Solution solution, Dependency dependency)
        {
            return NugetSearch.FindLatestByName(solution, dependency.Name);
        }
    }
}