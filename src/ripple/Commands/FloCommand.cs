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

        public bool VerboseFlag { get; set; }
    
        public LocalInput ToLocalInput()
        {
            return new LocalInput(){
                DirectFlag = true,
                FastFlag = true,
                FromFlag = From,
                ToFlag = To,
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