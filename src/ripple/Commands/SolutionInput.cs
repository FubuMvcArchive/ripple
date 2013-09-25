using System;
using System.Collections.Generic;
using ripple.Model;

namespace ripple.Commands
{
    public interface IOverrideFeeds
    {
        IEnumerable<Feed> Feeds();
    }

    public class SolutionInput : RippleInput
    {
        public void EachSolution(Action<Solution> configure)
        {
            var solutions = FindSolutions();
            solutions.Each(solution =>
            {
                RippleLog.Debug("Solution " + solution.Name);
                configure(solution);
            });
        }

        public IEnumerable<Solution> FindSolutions()
        {
            if (RippleFileSystem.IsSolutionDirectory())
            {
                yield return SolutionBuilder.ReadFromCurrentDirectory();
            }
        }
    }
}