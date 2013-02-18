using System.ComponentModel;
using FubuCore.CommandLine;

namespace ripple.Commands
{
    public class GitIgnoreInput : RippleInput
    {
        [Description("A line of text to add to a .gitignore file")]
        public string Line { get; set; }
    }
}