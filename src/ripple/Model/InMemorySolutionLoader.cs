using FubuCore;
using ripple.Model.Conditions;

namespace ripple.Model
{
    // Mostly used for testing
    public class InMemorySolutionLoader : ISolutionLoader
    {
        private readonly Solution _solution;

        public InMemorySolutionLoader(Solution solution)
        {
            _solution = solution;
        }

        public IDirectoryCondition Condition { get { return new Always(); } }

        public Solution LoadFrom(IFileSystem fileSystem, string file)
        {
            _solution.Directory = RippleFileSystem.CurrentDirectory();
            return _solution;
        }

        public void SolutionLoaded(Solution solution)
        {
        }
    }
}