using System;
using System.Collections.Generic;
using System.Reflection;
using FubuCore;
using ripple.Model;

namespace ripple.Local
{
    public class RippleStepRunner : IRippleStepRunner
    {
        private readonly IFileSystem _fileSystem;
        private readonly IRippleLogger _logger;
        private readonly RipplePlanRequirements _requirements;
        private readonly IProcessRunner _runner;


        public RippleStepRunner(IProcessRunner runner, IFileSystem fileSystem, IRippleLogger logger, RipplePlanRequirements requirements)
        {
            _runner = runner;
            _fileSystem = fileSystem;
            _logger = logger;
            _requirements = requirements;
        }

        public void BuildSolution(Solution solution)
        {
            var process = solution.CreateBuildProcess(_requirements.Fast);
            var processReturn = _runner.Run(process);

            _fileSystem.WriteLogFile(solution.Name + ".log", processReturn.OutputText);
            
            if (processReturn.ExitCode != 0)
            {
                throw new ApplicationException("Command line execution failed!!!!");
            } 
        }

        public void CopyFiles(FileCopyRequest request)
        {
            var files = _fileSystem.FindFiles(request.From, request.Matching);
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