using System;
using System.ComponentModel;
using FubuCore.CommandLine;
using System.Collections.Generic;
using FubuCore;
using System.Linq;
using ripple.Local;

namespace ripple.Commands
{
    public class UpdateInput : SolutionInput
    {
        [Description("Only update a specific nuget by name")]
        public string NugetFlag { get; set; }
    }

    [CommandDescription("Update nuget versions for solutions")]
    public class UpdateCommand : FubuCommand<UpdateInput>
    {
        public override bool Execute(UpdateInput input)
        {
            var command = "update {0} -r {1}";
            Func<Project, bool> projectFilter = p => p.NugetDependencies.Any();

            if (input.NugetFlag.IsNotEmpty())
            {
                command += " -i " + input.NugetFlag;
                projectFilter = p => p.NugetDependencies.Any(x => x.Name == input.NugetFlag);
            }

            input.FindSolutions().Each(solution =>
            {
                solution.Projects.Where(projectFilter).Each(project =>
                {
                    CLIRunner.RunNuget(command, project.PackagesFile(), solution.PackagesFolder());
                });
            });

            var listInput = new ListInput(){
                SolutionFlag = input.SolutionFlag,
                AllFlag = input.AllFlag,
                Mode = ListMode.dependencies
            };

            new ListCommand().Execute(listInput);

            return true;
        }
    }
}