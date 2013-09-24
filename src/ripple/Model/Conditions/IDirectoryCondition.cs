using FubuCore;

namespace ripple.Model.Conditions
{
    public interface IDirectoryCondition
    {
        bool Matches(IFileSystem fileSystem, string directory);
    }
}