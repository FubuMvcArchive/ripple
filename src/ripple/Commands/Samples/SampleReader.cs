using System;
using System.Collections.Generic;
using FubuCore;

namespace ripple.Commands.Samples
{
    public class SampleReader
    {
        private readonly string _codeFolder;
        private readonly string _outputFolder;
        private readonly IFileSystem _fileSystem = new FileSystem();

        public SampleReader(string codeFolder, string outputFolder)
        {
            _codeFolder = codeFolder;
            _outputFolder = outputFolder;
        }

        public void FindSamples()
        {
            if (!_fileSystem.DirectoryExists(_outputFolder))
            {
                Console.WriteLine("Creating folder " + _outputFolder);
                _fileSystem.CreateDirectory(_outputFolder);
            }
            else
            {
                Console.WriteLine("Cleaning out old samples at " + _outputFolder);
                _fileSystem.CleanDirectory(_outputFolder);
            }

            ReadDirectory(_codeFolder);
        }

        public void ReadDirectory(string directory)
        {
            Console.WriteLine("Searching directory {0} for samples", directory);
            var system = new FileSystem();
            system.FindFiles(directory, new FileSet{
                DeepSearch = true,
                Include = "*.cs"
            })
                .Each(readFile);
        }

        private void readFile(string file)
        {
            Console.WriteLine("  reading " + file);
            var collector = new FileCollector(file);
            collector.Read().Each(x => x.WriteFile(_outputFolder));
        }
    }
}