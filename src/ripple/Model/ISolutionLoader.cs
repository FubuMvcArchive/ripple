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
			//no-op
		}
	}
}