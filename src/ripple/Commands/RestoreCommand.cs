using System;
using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using System.Collections.Generic;
using ripple.Model;
using ripple.Nuget;
using System.Linq;

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

            var packagesFolder = solution.PackagesFolder();
            new FileSystem().CreateDirectory(packagesFolder);

            var nugetService = new NugetService(solution);
            solution.GetAllNugetDependencies().OrderBy(x => x.Name).Each(dep => nugetService.Install(dep));
        }
    }
}