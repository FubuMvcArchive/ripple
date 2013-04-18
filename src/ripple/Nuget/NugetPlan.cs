using System.Collections;
using System.Collections.Generic;

namespace ripple.Nuget
{
    public class NugetPlan : IEnumerable<INugetStep>
    {
        private readonly IList<INugetStep> _steps = new List<INugetStep>();

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

        public void Import(NugetPlan plan)
        {
            plan.Each(AddStep);
        }
    }
}