using FubuCore;
using ripple.Model.Conditions;

namespace ripple.Model
{
    public interface ISolutionLoader
    {
        IDirectoryCondition Condition { get; }

        Solution LoadFrom(IFileSystem fileSystem, string directory);
        void SolutionLoaded(Solution solution);

        //IDependencyStrategy DetermineStrategy();
    }
}