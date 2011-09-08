using System;
using System.Collections.Generic;
using FubuCore.DependencyAnalysis;
using FubuCore.Util;
using System.Linq;
using FubuCore;

namespace ripple
{
    public class SolutionGraph
    {
        private readonly Lazy<IEnumerable<NugetSpec>> _allNugets;
        private readonly Cache<string, Solution> _solutions = new Cache<string, Solution>(key =>
        {
            throw new InvalidSolutionException(key);
        });
        private readonly Lazy<IList<Solution>> _orderedSolutions;

        public SolutionGraph(IEnumerable<Solution> solutions)
        {
            _allNugets = new Lazy<IEnumerable<NugetSpec>>(() => _solutions.SelectMany(x => x.PublishedNugets).ToList());

            solutions.Each(s => _solutions[s.Name] = s);
            solutions.Each(s => s.DetermineDependencies(FindNugetSpec));
        
            _orderedSolutions = new Lazy<IList<Solution>>(() =>
            {
                var graph = new DependencyGraph<Solution>(s => s.Name, s => s.SolutionDependencies().Select(x => x.Name));
                solutions.Each(graph.RegisterItem);
                return graph.Ordered().ToList();
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
                return _orderedSolutions.Value;
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

    public class InvalidSolutionException : Exception
    {
        public InvalidSolutionException(string solutionName)
            : base("Solution {0} does not exist".ToFormat(solutionName))
        {
        }
    }
}