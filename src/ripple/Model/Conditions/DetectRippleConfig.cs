using FubuCore;

namespace ripple.Model.Conditions
{
    public class DetectRippleConfig : IDirectoryCondition
    {
        public bool Matches(IFileSystem fileSystem, string directory)
        {
            return RippleFileSystem.IsSolutionDirectory(directory);
        }

        public override bool Equals(object obj)
        {
            return obj is DetectRippleConfig;
        }
    }
}