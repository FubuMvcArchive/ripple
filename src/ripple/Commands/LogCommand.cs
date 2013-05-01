using System.ComponentModel;
using FubuCore.CommandLine;
using FubuCore;

namespace ripple.Commands
{
    public class LogInput
    {
        [Description("Specify the name of a solution log to open")]
        public string SolutionFlag { get; set; }

        [Description("Open the ripple log for the solution")]
        [FlagAlias("open", 'o')]
        public bool OpenFlag { get; set; }

        [Description("Truncate the ripple log for the solution")]
        [FlagAlias("truncate", 't')]
        public bool TruncateFlag { get; set; }
    }

    [CommandDescription("Modifies the ripple log file")]
    public class LogCommand : FubuCommand<LogInput>
    {
        public LogCommand()
        {
            Usage("Opens the log file for the solution").Arguments(x => x.OpenFlag);
            Usage("Truncates the log file for the solution").Arguments(x => x.TruncateFlag);
        }

        public override bool Execute(LogInput input)
        {
            var fileSystem = new FileSystem();
            var logFile = input.SolutionFlag.IsEmpty() ? RippleLog.FileListener.File : input.SolutionFlag;

            if (input.OpenFlag)
            {
                fileSystem.OpenLogFile(logFile);
                return true;
            }

            fileSystem.TruncateLogFile(logFile);
            return true;
        }
    }
}