using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace ripple
{
    public class RipplePlanRequirements
    {
        // If not set, use all
        public string From { get; set; }
        public string To { get; set; }
        public bool Fast { get; set; }
        public bool Direct { get; set; }

        public IEnumerable<Solution> SelectSolutions(SolutionGraph theGraph)
        {
            var start = From.IsEmpty() ? null : theGraph[From];
            var end = To.IsEmpty() ? null : theGraph[To];

            if (Direct)
            {
                if (To.IsEmpty() || From.IsEmpty())
                {
                    throw new InvalidOperationException("If using the Direct option, you must specify bith a From and To solution");
                }

                return new [] { start, end };
            }

            var solutions = theGraph.AllSolutions.ToList();
            solutions = filterByStarting(start, solutions);
            solutions = filterByEnding(end, solutions);

            return solutions;
        }

        private static List<Solution> filterByEnding(Solution end, List<Solution> solutions)
        {
            if (end != null)
            {
                solutions = solutions.TakeWhile(x => x != end).Where(end.DependsOn).ToList();
                solutions.Add(end);
            }
            return solutions;
        }

        private static List<Solution> filterByStarting(Solution start, List<Solution> solutions)
        {
            if (start != null)
            {
                solutions = solutions
                    .SkipWhile(x => x != start)
                    .Where(x => x == start || x.DependsOn(start))
                    .ToList();
            }
            return solutions;
        }
    }
}