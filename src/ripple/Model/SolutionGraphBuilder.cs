using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace ripple.Model
{
    public class SolutionGraphBuilder
    {
        public static SolutionGraph BuildForCurrentDirectory()
        {
            var builder = new SolutionGraphBuilder(new FileSystem());
            var codeDirectory = RippleFileSystem.FindCodeDirectory();

            return builder.ReadFrom(codeDirectory);
        }

        private readonly IFileSystem _fileSystem;

        public SolutionGraphBuilder(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public SolutionGraph ReadFrom(string folder)
        {
            folder = findCorrectFolder(folder);

            RippleLog.Info("Trying to read a Ripple SolutionGraph from " + folder);

            var solutions = readSolutions(folder);

            return new SolutionGraph(solutions);
        }

        private static string findCorrectFolder(string folder)
        {
            var config = SolutionConfig.LoadFrom(folder);
            if (config != null)
            {
                folder = folder.ParentDirectory();
            }

            return folder;
        }

        private IEnumerable<Solution> readSolutions(string folder)
        {
            return _fileSystem.ChildDirectoriesFor(folder)
				.Where(x => _fileSystem.FileExists(x.AppendPath(SolutionFiles.ConfigFile)))
                .Select(SolutionBuilder.ReadFrom)
                .ToList();
        }


    }
}