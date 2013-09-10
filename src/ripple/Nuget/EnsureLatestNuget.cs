using ripple.Model;

namespace ripple.Nuget
{
    public class EnsureLatestNuget : INugetFilter
    {
        public void Filter(Solution solution, Dependency dependency, NugetResult result)
        {
            if (!dependency.IsFloat() || !result.Found) return;

            var latest = NugetSearch.FindLatestByName(solution, dependency.Name);
            if (!latest.Found)
            {
                return;
            }

            if (latest.Nuget.Version > result.Nuget.Version)
            {
                result.Import(latest);
            }
        }
    }
}