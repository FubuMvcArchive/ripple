using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using ripple.New.Model;

namespace ripple.Model
{
    public class SolutionGraphBuilder
    {
        public static SolutionGraph BuildForRippleDirectory()
        {
            var builder = new SolutionGraphBuilder(new FileSystem());
            var codeDirectory = RippleFileSystem.CodeDirectory();

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

            Console.WriteLine("Trying to read a Ripple SolutionGraph from " + folder);

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