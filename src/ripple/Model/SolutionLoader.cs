using FubuCore;
using ripple.Model.Conditions;

namespace ripple.Model
{
    public class SolutionLoader : ISolutionLoader
    {
        public IDirectoryCondition Condition { get; private set; }

        public Solution LoadFrom(IFileSystem fileSystem, string directory)
        {
            return fileSystem.LoadFromFile<Solution>(directory);
        }

        public void SolutionLoaded(Solution solution)
        {
        }
    }
}