using System;

namespace ripple.Model
{
    public interface ISolutionFiles
    {
        string RootDir { get; }

        void ForProjects(Solution solution, Action<string> action);
        void ForNuspecs(Solution solution, Action<string> action);

        Solution LoadSolution();

        void FinalizeSolution(Solution solution);
    }
}
