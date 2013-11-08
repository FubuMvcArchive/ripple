using System.Collections.Generic;
using System.Linq;
using FubuCore.Descriptions;
using FubuCore.Logging;

namespace ripple.Model
{
    public class ValidationResult : DescribesItself, LogTopic
    {
        private readonly Solution _solution;
        private readonly IList<RippleProblem> _problems = new List<RippleProblem>();
 
        public ValidationResult(Solution solution) 
        {
            _solution = solution;
        }

        public void AddProblem(string provenance, string problem)
        {
            AddProblem(new RippleProblem(provenance, problem));
        }

        public void AddProblem(RippleProblem problem)
        {
            _problems.Fill(problem);
        }

        public bool IsValid()
        {
            return !_problems.Any();
        }

        public void Import(DependencyValidationResult result)
        {
            result.Problems.Each(AddProblem);
        }

        public IEnumerable<RippleProblem> Problems { get { return _problems; } } 

        public void Describe(Description description)
        {
            description.Title = _solution.Name;

            if (IsValid())
            {
                description.ShortDescription = "No problems found";
                return;
            }

            description.ShortDescription = "Problems were found for " + _solution.Name;

            var list = description.AddList("Problems", _problems);
            list.Label = "Problems";
        }
    }
}