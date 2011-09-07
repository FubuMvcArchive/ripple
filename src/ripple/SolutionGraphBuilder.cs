using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace ripple
{
    public class SolutionGraphBuilder
    {
        private readonly IFileSystem _fileSystem;

        public SolutionGraphBuilder(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public SolutionGraph ReadFrom(string folder)
        {
            folder = findCorrectFolder(folder);
            var solutions = readSolutions(folder);

            return new SolutionGraph(solutions);
        }

        private static string findCorrectFolder(string folder)
        {
            var config = SolutionConfig.LoadFrom(folder);
            if (config != null)
            {
                // TODO -- get a decent Extension method in FubuCore for this
                folder = Path.GetDirectoryName(folder);
            }
            return folder;
        }

        private IEnumerable<Solution> readSolutions(string folder)
        {
            return _fileSystem.FindFiles(folder, new FileSet{
                Include = SolutionConfig.FileName
            }).Select(file =>
            {
                var dir = Path.GetDirectoryName(file);
                return Solution.ReadFrom(dir);

            }).ToList();
        }


    }
}