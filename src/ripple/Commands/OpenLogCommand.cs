using System;
using System.ComponentModel;
using FubuCore.CommandLine;
using FubuCore;

namespace ripple.Commands
{
    public class OpenLogInput
    {
        [Description("Specify the name of a solution log to open")]
        [RequiredUsage("solution")]
        public string Solution { get; set; }
    }

    [Usage("solution", "Opens the build log file for the named solution")]
    [Usage("all", "Opens the log for the local ripple runner")]
    [CommandDescription("Opens up the log file for the last local ripple run", Name = "open-log")]
    public class OpenLogCommand : FubuCommand<OpenLogInput>
    {
        public override bool Execute(OpenLogInput input)
        {
            var logFile = input.Solution.IsEmpty() ? "ripple.log" : input.Solution;
            new FileSystem().OpenLogFile(logFile);

            return true;
        }
    }
}