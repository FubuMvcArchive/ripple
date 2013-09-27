using FubuCore;

namespace ripple.Model
{
    public interface ISolutionLoader
    {
        bool CanLoad(IFileSystem fileSystem, string directory);
        Solution LoadFrom(IFileSystem fileSystem, string filePath);
        void SolutionLoaded(Solution solution);
    }
}