using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;
using FubuCore.Logging;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Steps
{
    public class MissingNugetReport : LogTopic, DescribesItself
    {
        private readonly IList<Dependency> _nugets = new List<Dependency>();
        private readonly IList<NugetProblem> _problems = new List<NugetProblem>();

        public void Add(Dependency dependency)
        {
            _nugets.Add(dependency);
        }

        public void AddProblems(IEnumerable<NugetProblem> problems)
        {
            _problems.AddRange(problems);
        }

        public bool IsValid()
        {
            return !_nugets.Any();
        }

        public void Describe(Description description)
        {
            description.Title = "Missing Nugets";
            description.ShortDescription = "Could not find nugets";
            description.AddList("Nugets", _nugets);

            if (_problems.Any())
            {
                description.AddList("Problems", _problems);
            }
        }
    }
}