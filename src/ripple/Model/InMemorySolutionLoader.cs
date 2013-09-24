using FubuCore;
using ripple.Model.Conditions;

namespace ripple.Model
{
    public class InMemorySolutionLoader : ISolutionLoader
    {
        private readonly Solution _solution;

        public InMemorySolutionLoader(Solution solution)
        {
            _solution = solution;
        }

        public IDirectoryCondition Condition { get; private set; }

        public Solution LoadFrom(IFileSystem fileSystem, string directory)
        {
            _solution.Directory = RippleFileSystem.CurrentDirectory();
            return _solution;
        }

        public void SolutionLoaded(Solution solution)
        {
        }
    }
}