using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;
using FubuCore.Logging;

namespace ripple.Model
{
    public class DependencyValidationResult : DescribesItself, LogTopic
    {
        private readonly IList<RippleProblem> _problems = new List<RippleProblem>();

        public void AddProblem(string provenance, string problem)
        {
            _problems.Fill(new RippleProblem(provenance, problem));
        }

        public bool IsValid()
        {
            return !_problems.Any();
        }

        public IEnumerable<RippleProblem> Problems { get { return _problems; } }

        public void Describe(Description description)
        {
            description.Title = "Dependency Validation";

            if (IsValid())
            {
                description.ShortDescription = "No problems detected";
                return;
            }

            description.ShortDescription = "Problems were detected";

            var list = description.AddList("Problems", _problems);
            list.Label = "Problems";
        }
    }
}