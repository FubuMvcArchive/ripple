using System;
using System.ComponentModel;
using FubuCore.CommandLine;
using System.Collections.Generic;
using ripple.Model;

namespace ripple.Commands
{


    [CommandDescription("Interacts with nuget to restore all the nuget dependencies in a solution tree")]
    public class RestoreCommand : FubuCommand<SolutionInput>
    {
        public override bool Execute(SolutionInput input)
        {
            input.FindSolutions().Each(restoreSolution);

            return true;
        }

        private static void restoreSolution(Solution solution)
        {
            Console.WriteLine("Restoring nuget dependencies for solution " + solution.Name);

            var solutionFolder = solution.PackagesFolder();


            solution.Projects.Each(p =>
            {
                var projectFile = p.PackagesFile();
                CLIRunner.RunNuget("i {0} -o {1}", projectFile, solutionFolder);
            });
        }
    }
}