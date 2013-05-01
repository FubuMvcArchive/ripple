using System;
using FubuCore.CommandLine;
using ripple.Model;

namespace ripple.Commands
{
    public class TestCommand : FubuCommand<SolutionInput>
    {
        public override bool Execute(SolutionInput input)
        {
            Console.WriteLine(BranchDetector.Current());
            return true;
        }
    }
}