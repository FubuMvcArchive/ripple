using System.Linq;
using FubuCore;

namespace ripple.Model.Conditions
{
    public class DetectSingleSolution : IDirectoryCondition
    {
        public bool Matches(IFileSystem fileSystem, string directory)
        {
            return FindSolutionFile(fileSystem, directory).IsNotEmpty();
        }

        public static string FindSolutionFile(IFileSystem fileSystem, string directory)
        {
            var slnSet = new FileSet
            {
                Include = "*.sln",
                DeepSearch = true
            };

            var files = fileSystem.FindFiles(directory, slnSet).ToArray();
            return files.Length != 1 ? null : files[0];
        }

        public override bool Equals(object obj)
        {
            return obj is DetectSingleSolution;
        }
    }
}