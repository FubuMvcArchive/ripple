using System.IO;
using FubuCore;

namespace ripple.Nuget
{
    public class PublishedAssembly
    {
        private readonly string _subFolder;
        private readonly string _directory;
        private readonly string _name;
        private readonly string _pattern;

        public PublishedAssembly(string nuspecDirectory, string assemblyReference, string subFolder)
        {
            _subFolder = subFolder;
            _name = Path.GetFileNameWithoutExtension(assemblyReference);

            _directory = nuspecDirectory.AppendPath(Path.GetDirectoryName(assemblyReference)).ToFullPath();

            _pattern = Path.GetFileName(assemblyReference);
        }

        public string Name
        {
            get { return _name; }
        }

        public string Pattern
        {
            get { return _pattern; }
        }

        public string Directory
        {
            get { return _directory; }
        }

        public string SubFolder
        {
            get { return _subFolder; }
        }
    }
}