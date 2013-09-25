using FubuCore;

namespace ripple.Model.Conditions
{
    public class Always : IDirectoryCondition
    {
        public bool Matches(IFileSystem fileSystem, string directory)
        {
            return true;
        }
    }
}