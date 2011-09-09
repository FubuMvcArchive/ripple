using System.ComponentModel;
using FubuCore.CommandLine;

namespace ripple.Commands
{
    public class GitIgnoreInput : RippleInput
    {
        [RequiredUsage("add")]
        [Description("A line of text to add to a .gitignore file")]
        public string Line { get; set; }
    }
}