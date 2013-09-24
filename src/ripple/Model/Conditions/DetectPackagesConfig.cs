using System.Linq;
using FubuCore;

namespace ripple.Model.Conditions
{
    public class DetectPackagesConfig : IDirectoryCondition
    {
        public const string ConfigFile = "packages.config";

        public bool Matches(IFileSystem fileSystem, string directory)
        {
            var packageConfigs = new FileSet
            {
                Include = ConfigFile,
                DeepSearch = true
            };

            return fileSystem.FindFiles(directory, packageConfigs).Any();
        }

        public override bool Equals(object obj)
        {
            return obj is DetectPackagesConfig;
        }
    }
}