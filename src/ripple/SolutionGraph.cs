using System;
using System.Collections.Generic;
using FubuCore.Util;
using System.Linq;

namespace ripple
{
    public class SolutionGraph
    {
        private readonly Cache<string, Solution> _solutions = new Cache<string, Solution>();

        public SolutionGraph(IEnumerable<Solution> solutions)
        {
            solutions.Each(s =>
            {
                _solutions[s.Name] = s;
            });
        }

        public Solution this[string name]
        {
            get
            {
                return _solutions[name];
            }
            set
            {
                _solutions[name] = value;
            }
        }


        public IEnumerable<Solution> AllSolutions
        {
            get
            {
                return _solutions.GetAll();
            }
        }

        public NugetSpec FindNugetSpec(string nugetName)
        {
            return AllNugets().FirstOrDefault(x => x.Name == nugetName);
        }

        public IEnumerable<NugetSpec> AllNugets()
        {
            return _solutions.SelectMany(x => x.PublishedNugets);
        }

        public IEnumerable<NugetSpec> FindFromDependencies(IEnumerable<NugetDependency> dependencies)
        {
            return AllNugets().Where(x => dependencies.Any(d => d.Name == x.Name));
        }
    }
}