using FubuCore;
using System.Linq;

namespace ripple.MSBuild
{
    public class Reference
    {
        public string Name { get; set; }
        public string HintPath { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, HintPath: {1}", Name, HintPath);
        }

        public bool Matches(string assemblyName)
        {
            if (Name.EqualsIgnoreCase(assemblyName)) return true;

            var guessedName = Name.Split(',').First().Trim();

            return assemblyName.EqualsIgnoreCase(guessedName);

        }
    }
}