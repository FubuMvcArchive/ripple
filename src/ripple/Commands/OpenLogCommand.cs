using System.ComponentModel;
using FubuCore.CommandLine;
using FubuCore;

namespace ripple.Commands
{
    public class OpenLogInput
    {
        [Description("Specify the name of a solution log to open")]
        public string Solution { get; set; }
    }

    [CommandDescription("Opens up the log file for the last local ripple run", Name = "open-log")]
    public class OpenLogCommand : FubuCommand<OpenLogInput>
    {
	    public OpenLogCommand()
	    {
		    Usage("Opens the build log file for the named solution").Arguments(x => x.Solution);
		    Usage("Opens the log for the local ripple runner");
	    }

        public override bool Execute(OpenLogInput input)
        {
            var logFile = input.Solution.IsEmpty() ? "ripple.log" : input.Solution;
            new FileSystem().OpenLogFile(logFile);

            return true;
        }
    }
}