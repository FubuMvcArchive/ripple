using System.Collections;
using System.Collections.Generic;
using FubuCore.Descriptions;
using FubuCore.Logging;

namespace ripple.Nuget
{
    public class NugetPlan : IEnumerable<INugetStep>, DescribesItself, LogTopic
    {
        private readonly IList<INugetStep> _steps = new List<INugetStep>();

        public NugetPlan()
        {
        }

        public NugetPlan(params INugetStep[] steps)
        {
            _steps.Fill(steps);
        }

        public void AddStep(INugetStep step)
        {
            _steps.Fill(step);
        }

        public IEnumerator<INugetStep> GetEnumerator()
        {
            return _steps.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Execute(INugetStepRunner runner)
        {
            this.Each(step => step.Execute(runner));
        }

        public void Import(NugetPlan plan)
        {
            plan.Each(AddStep);
        }

        public void Describe(Description description)
        {
            description.ShortDescription = "";
            description.AddList("Steps", _steps);
        }
    }
}