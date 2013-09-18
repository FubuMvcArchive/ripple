using FubuCore;

namespace ripple.Model
{
    public class SolutionLoader : ISolutionLoader
    {
        public bool CanLoad(IFileSystem fileSystem, string directory)
        {
            throw new System.NotImplementedException();
        }

        public Solution LoadFrom(IFileSystem fileSystem, string filePath)
        {
            return fileSystem.LoadFromFile<Solution>(filePath);
        }

        public void SolutionLoaded(Solution solution)
        {
        }
    }
}