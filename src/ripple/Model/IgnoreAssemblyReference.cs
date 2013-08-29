using FubuCore;

namespace ripple.Model
{
    public class IgnoreAssemblyReference
    {
        public string Package { get; set; }
        public string Assembly { get; set; }


        public bool Matches(Dependency dependency, string assembly)
        {
            return dependency.MatchesName(Package) && Assembly.Replace(".dll", "").EqualsIgnoreCase(assembly);
        }
    }
}