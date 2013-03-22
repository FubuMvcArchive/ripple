using System;
using FubuCore;
using FubuCore.CommandLine;

namespace ripple.Commands
{
    public class OpenRakeInput : SolutionInput
    {
        
    }

    [CommandDescription("Open the rake file (assuming it exists) for the current solution or a named solution", Name = "open-rake")]
    public class OpenRakeCommand : FubuCommand<OpenRakeInput>
    {
        public override bool Execute(OpenRakeInput input)
        {
            var system = new FileSystem();
            input.EachSolution(x =>
            {
                var rakeFile = x.Directory.AppendPath("rakefile.rb");
                if (system.FileExists(rakeFile))
                {
                    Console.WriteLine("Opening " + rakeFile);
                    system.LaunchEditor(rakeFile);
                }
            });

            return true;
        }
    }
}