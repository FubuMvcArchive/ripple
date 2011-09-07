using System;
using System.Collections.Generic;
using FubuCore.Util;
using System.Linq;

namespace ripple
{
    public class SolutionGraph
    {
        private readonly Lazy<IEnumerable<NugetSpec>> _allNugets;
        private readonly Cache<string, Solution> _solutions = new Cache<string, Solution>();

        public SolutionGraph(IEnumerable<Solution> solutions)
        {
            _allNugets = new Lazy<IEnumerable<NugetSpec>>(() => _solutions.SelectMany(x => x.PublishedNugets).ToList());

            solutions.Each(s => _solutions[s.Name] = s);
            solutions.Each(s => s.DetermineDependencies(FindNugetSpec));
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
            return _allNugets.Value;
        }

        public IEnumerable<NugetSpec> FindFromDependencies(IEnumerable<NugetDependency> dependencies)
        {
            return AllNugets().Where(x => dependencies.Any(d => d.Name == x.Name));
        }
    }
}