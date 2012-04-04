using System;
using FubuCore;
using ripple.Model;
using System.Collections.Generic;

namespace ripple.Directives
{
    public class DirectiveProcessor
    {
        public static void ProcessDirectives(ISolution solution)
        {
            var fileSystem = new FileSystem();
            var processor = new DirectiveProcessor(fileSystem, solution, new DirectiveRunner(fileSystem, solution),
                                                   new DirectiveParser());

            processor.ProcessAll();
        }

        private readonly IFileSystem _fileSystem;
        private readonly ISolution _solution;
        private readonly IDirectiveRunner _runner;
        private readonly IDirectiveParser _parser;

        public DirectiveProcessor(IFileSystem fileSystem, ISolution solution, IDirectiveRunner runner, IDirectiveParser parser)
        {
            _fileSystem = fileSystem;
            _solution = solution;
            _runner = runner;
            _parser = parser;
        }

        public void ProcessAll()
        {
            _solution.AllNugetDependencyNames().Each(name =>
            {
                var directory = _solution.NugetFolderFor(name);
                ProcessNuget(directory);
            });
        }

        private void ProcessNuget(string directory)
        {
            var files = _fileSystem.FindFiles(directory, new FileSet{
                DeepSearch = true,
                Include = "ripple.xml"
            });

            files.Each(file => ProcessDirectives(file, directory));
        }

        private void ProcessDirectives(string file, string directory)
        {
            Console.WriteLine("Processing directives at " + file);

            var relative = file.PathRelativeTo(directory);
            _runner.SetCurrentDirectory(directory, relative);

            _parser.Read(file, _runner);
        }
    }
}