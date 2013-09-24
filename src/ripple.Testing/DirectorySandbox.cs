using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using ripple.Commands;

namespace ripple.Testing
{
    public class DirectorySandbox : IDisposable
    {
        private readonly string _directory;
        private readonly IFileSystem _fileSystem;

        public DirectorySandbox(string directory)
        {
            _directory = directory;
            _fileSystem = new FileSystem();

            RippleFileSystem.StubCurrentDirectory(directory);
            RippleFileSystem.StopTraversingAt(directory);
        }

        public string Directory { get { return _directory.ToFullPath(); } }

        public void CreateDirectory(params string[] parts)
        {
            var combined = new List<string> {_directory};
            combined.AddRange(parts);

            _fileSystem.CreateDirectory(combined.ToArray());
        }

        public void CreateFile(params string[] parts)
        {
            var file = _directory.AppendPath(parts);
            _fileSystem.WriteStringToFile(file, "");
        }

        public string FindDirectory(params string[] parts)
        {
            return _directory.AppendPath(parts).ToFullPath();
        }

        public void StopAtParent()
        {
            RippleFileSystem.StopTraversingAt(_directory.ParentDirectory());
        }

        public void Dispose()
        {
            _fileSystem.ForceClean(_directory);
            RippleFileSystem.Live();
        }

        public static DirectorySandbox Create()
        {
            return new DirectorySandbox(Path.GetTempPath().AppendRandomPath());
        }
    }
}