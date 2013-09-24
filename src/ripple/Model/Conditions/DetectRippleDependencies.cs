using System.Linq;
using FubuCore;

namespace ripple.Model.Conditions
{
    public class DetectRippleDependencies : IDirectoryCondition
    {
        public const string ConfigFile = "ripple.dependencies.config";

        public bool Matches(IFileSystem fileSystem, string directory)
        {
            var rippleConfigs = new FileSet
            {
                Include = ConfigFile,
                DeepSearch = true
            };

            return fileSystem.FindFiles(directory, rippleConfigs).Any();
        }

        public override bool Equals(object obj)
        {
            return obj is DetectRippleDependencies;
        }
    }
}