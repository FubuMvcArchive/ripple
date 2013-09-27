using FubuCore;

namespace ripple.Model
{
    public class InMemorySolutionLoader : ISolutionLoader
    {
        private readonly Solution _solution;

        public InMemorySolutionLoader(Solution solution)
        {
            _solution = solution;
        }

        public bool CanLoad(IFileSystem fileSystem, string directory)
        {
            return true;
        }

        public Solution LoadFrom(IFileSystem fileSystem, string filePath)
        {
            _solution.Directory = RippleFileSystem.CurrentDirectory();
            return _solution;
        }

        public void SolutionLoaded(Solution solution)
        {
        }
    }
}