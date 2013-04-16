using System;
using System.Collections.Generic;
using FubuCore;
using FubuCore.DependencyAnalysis;
using FubuCore.Descriptions;
using FubuCore.Util;
using System.Linq;
using ripple.Local;

namespace ripple.Model
{
    public class SolutionGraph : DescribesItself
    {
        private readonly Lazy<IEnumerable<NugetSpec>> _allNugets;
		private readonly Lazy<IList<Solution>> _orderedSolutions;
		private readonly Cache<string, Solution> _solutions = new Cache<string, Solution>(key =>
        {
            throw new InvalidSolutionException(key);
        });

        public SolutionGraph(IEnumerable<Solution> solutions)
        {
            _allNugets = new Lazy<IEnumerable<NugetSpec>>(() => _solutions.SelectMany(x => x.Specifications).ToList());

            solutions.Each(s => _solutions[s.Name] = s);
			solutions.Each(s => s.DetermineNugetDependencies(FindNugetSpec));
        
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

        public IEnumerable<NugetSpec> FindFromDependencies(IEnumerable<Dependency> dependencies)
        {
            return AllNugets().Where(x => dependencies.Any(d => d.Name == x.Name));
        }

        public void Describe(Description description)
        {
            description.Title = "Solution Graph at {0}".ToFormat(RippleFileSystem.FindCodeDirectory());

            var solutions = description.AddList("Solutions", _orderedSolutions.Value.Select(x => new SolutionListItem(x)));
            solutions.Label = "Solutions";
        }

        public class SolutionListItem : DescribesItself
        {
            private readonly Solution _solution;

            public SolutionListItem(Solution solution)
            {
                _solution = solution;
            }

            public void Describe(Description description)
            {
                description.Title = _solution.Name;
                description.ShortDescription = _solution.Path;
            }
        }
    }
}