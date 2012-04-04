using System;
using FubuCore;
using ripple.Model;

namespace ripple.Directives
{
    public class DirectiveRunner : IDirectiveRunner
    {
        private readonly IFileSystem _fileSystem;
        private readonly ISolution _solution;
        private string _current;
        private string _relativeFromNuget;

        public DirectiveRunner(IFileSystem fileSystem, ISolution solution)
        {
            _fileSystem = fileSystem;
            _solution = solution;
        }

        public void SetCurrentDirectory(string current, string relativeFromNuget)
        {
            _current = current;
            _relativeFromNuget = relativeFromNuget;
        }

        public void CreateRunner(string file, string alias)
        {
            var text = "{0} %*".ToFormat(_current.AppendPath(file));
            var runnerName = alias + ".cmd";

            if (IsUnix())
            {
                runnerName = alias + ".sh";
                text = "ln -s #!/bin/sh {0} $*".ToFormat(file);
            }

            var runnerFile = _solution.Directory.AppendPath(runnerName);
            _fileSystem.WriteStringToFile(runnerFile, text);
            _solution.IgnoreFile(runnerName);
            Console.WriteLine("Writing file to " + runnerFile);
        }

        public void Copy(string file, string relativePath, string nuget)
        {
            var from = _current.AppendPath(file);

            var to = nuget.IsEmpty() ? _solution.Directory : _solution.NugetFolderFor(nuget);

            if (relativePath.IsEmpty() && nuget.IsNotEmpty())
            {
                relativePath = _relativeFromNuget;
            }

            if (relativePath.IsNotEmpty())
            {
                to = to.AppendPath(relativePath);
            }


            Console.WriteLine("Copying {0} to {1}", from, to);
            _fileSystem.Copy(from, to);
        }

        public static bool IsUnix()
        {
            var pf = Environment.OSVersion.Platform;
            return pf == PlatformID.Unix || pf == PlatformID.MacOSX;
        }
    }
}