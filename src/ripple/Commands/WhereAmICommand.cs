using System;
using FubuCore.CommandLine;

namespace ripple.Commands
{

    public class WhereAmIInput
    {
        
    }

    [CommandDescription("Tells you where ripple thinks the root folder is at", Name = "whereami")]
    public class WhereAmICommand : FubuCommand<WhereAmIInput>
    {
        public override bool Execute(WhereAmIInput input)
        {
            Console.WriteLine("The root code directory for ripple is " + RippleFileSystem.CodeDirectory());
            Console.WriteLine("ripple.exe is at " + RippleFileSystem.RippleExeLocation());

            return true;
        }
    }
}