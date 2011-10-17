using System;
using System.ComponentModel;
using FubuCore.CommandLine;
using System.Collections.Generic;

namespace ripple.Commands
{
    public class NugetInput : SolutionInput
    {
        [Description("The name of the nuget to allow to float")]
        public string Name { get; set; }
    }

    [CommandDescription("Allows a nuget to 'float' and be automatically updated from the Update command")]
    public class FloatCommand : FubuCommand<NugetInput>
    {
        public override bool Execute(NugetInput input)
        {
            input.FindSolutions().Each(solution =>
            {
                solution.AlterConfig(config => config.FloatNuget(input.Name));
            });

            return true;
        }
    }

    [CommandDescription("Locks a nuget to prevent it from being automatically updated from the Update command")]
    public class LockCommand : FubuCommand<NugetInput>
    {
        public override bool Execute(NugetInput input)
        {
            input.FindSolutions().Each(solution =>
            {
                solution.AlterConfig(config => config.LockNuget(input.Name));
            });

            return true;
        }
    }
}