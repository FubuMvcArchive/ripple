using System;
using System.ComponentModel;
using FubuCore.CommandLine;

namespace ripple.Commands
{
    public class FloInput
    {
        [Description("The origin")]
        public string From { get; set; }
 
        [Description("The destination")]
        public string To { get; set; }

		[Description("Use the 'no build' option to just shuffle binaries around (i.e., don't compile after moving)")]
		[FlagAlias("no-build", 'n')]
		public bool NoBuildFlag { get; set; }

		[Description("Writes out all the build output to the screen")]
		public bool VerboseFlag { get; set; }
    
        public LocalInput ToLocalInput()
        {
            return new LocalInput
	        {
                DirectFlag = true,
                FastFlag = true,
                FromFlag = From,
                ToFlag = To,
				NoBuildFlag = NoBuildFlag,
                VerboseFlag = VerboseFlag
            };
        }
    }

    [CommandDescription("Equivalent to a ripple local -from [from] -to [to] -fast -direct")]
    public class FloCommand : FubuCommand<FloInput>
    {
        public override bool Execute(FloInput input)
        {
            return new LocalCommand().Execute(input.ToLocalInput());
        }
    }
}