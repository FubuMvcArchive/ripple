using System.IO;
using FubuCore;

namespace ripple.Model.Conditions
{
    public class DetectXml : IDirectoryCondition
    {
        public bool Matches(IFileSystem fileSystem, string directory)
        {
            if (!RippleFileSystem.IsSolutionDirectory(directory))
            {
                return false;
            }

            var slnDir = RippleFileSystem.FindSolutionDirectory();
            var configFile = Path.Combine(slnDir, SolutionFiles.ConfigFile);

            if (!File.Exists(configFile)) return false;

            using (var stream = File.OpenRead(configFile))
            {
                return stream.ReadAllText().Contains("<?xml");
            }
        }

        public override bool Equals(object obj)
        {
            return obj is DetectXml;
        }
    }
}