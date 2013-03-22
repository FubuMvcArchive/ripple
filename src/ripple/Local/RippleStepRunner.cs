using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using FubuCore;
using ripple.Commands;
using ripple.Model;
using System.Linq;

namespace ripple.Local
{
    public class RippleStepRunner : IRippleStepRunner
    {
        private readonly IFileSystem _fileSystem;
        private readonly IRippleLogger _logger;
        private readonly RipplePlanRequirements _requirements;
        private readonly IProcessRunner _runner;
        private readonly Action<string> _logCallback;

        public RippleStepRunner(IProcessRunner runner, IFileSystem fileSystem, IRippleLogger logger, RipplePlanRequirements requirements)
        {
            _runner = runner;
            _fileSystem = fileSystem;
            _logger = logger;
            _requirements = requirements;

            _logCallback = requirements.Verbose ? (Action<string>) (text => _logger.Trace(text)) : text => { };
        }

        public void BuildSolution(Solution solution)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            if (solution.Dependencies.Any())
            {
                _logger.Trace("Pausing to try to let the file system quiet down...");
                Thread.Sleep(1000);
            }

            var process = solution.CreateBuildProcess(_requirements.Fast);
            _logger.Trace("Trying to run {0} {1} in directory {2}", process.FileName, process.Arguments, process.WorkingDirectory);

            ProcessReturn processReturn;
            _logger.Indent(() =>
            {
                processReturn = _runner.Run(process, new TimeSpan(0, 5, 0), _logCallback);

                _fileSystem.WriteLogFile(solution.Name + ".log", processReturn.OutputText);

                stopwatch.Stop();
                _logger.Trace("Completed in {0} milliseconds", stopwatch.ElapsedMilliseconds);

                if (processReturn.ExitCode != 0)
                {
                    _logger.Trace("Opening the log file for " + solution.Name);
                    new OpenLogCommand().Execute(new OpenLogInput()
                    {
                        Solution = solution.Name
                    });
                    throw new ApplicationException("Command line execution failed!!!!");
                } 
            });
            


        }

        public void CopyFiles(FileCopyRequest request)
        {
            _fileSystem.CreateDirectory(request.To);

            // Hack!
            if (!_fileSystem.DirectoryExists(request.From) && request.From.ToLower().Contains("release"))
            {
                var dir = request.From.ToLower().Replace("release", "debug");
                if (_fileSystem.DirectoryExists(dir))
                {
                    request.From = dir;
                }
            }

            var files = _fileSystem.FindFiles(request.From, request.Matching);
            if (!files.Any())
            {
                throw new ApplicationException("Unable to find any files matching {1} in {0}".ToFormat(request.From, request.Matching.Include));
            }

            files.Each(f =>
            {
                _logger.Trace("Copying {0} to {1}", f, request.To);
                _fileSystem.Copy(f, request.To);
            });
        }

        public void CleanDirectory(string directory)
        {
            Trace("Cleaning directory " + directory);
            _fileSystem.CleanDirectory(directory);
        }

        public void Trace(string format, params object[] parameters)
        {
            _logger.Trace(format, parameters);
        }

    }
}