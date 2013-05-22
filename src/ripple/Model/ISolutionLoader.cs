using FubuCore;

namespace ripple.Model
{
	public interface ISolutionLoader
	{
		Solution LoadFrom(IFileSystem fileSystem, string filePath);

		void SolutionLoaded(Solution solution);
	}

	public class SolutionLoader : ISolutionLoader
	{
		public Solution LoadFrom(IFileSystem fileSystem, string filePath)
		{
			return fileSystem.LoadFromFile<Solution>(filePath);
		}

		public void SolutionLoaded(Solution solution)
		{
		}
	}

    public class InMemorySolutionLoader : ISolutionLoader
    {
        private readonly Solution _solution;

        public InMemorySolutionLoader(Solution solution)
        {
            _solution = solution;
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