using System.Collections.Generic;
using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Local;
using ripple.Model;
using ripple.Steps;

namespace ripple.Commands
{
    public class LocalNugetInput : SolutionInput
    {
        public LocalNugetInput()
        {
            VersionFlag = "0.0.0.0";
            DestinationFlag = RippleFileSystem.LocalNugetDirectory();
        }

        [Description("Override the version of the nuget file")]
        public string VersionFlag { get; set; }

        [Description("Specify where the nuget file should be written, otherwise it just goes to the nuget default")]
        public string DestinationFlag { get; set; }

        [Description("Modify the nuspec files to match the current dependency version constraints")]
        [FlagAlias("update-dependencies", 'u')]
        public bool UpdateDependenciesFlag { get; set; }

        public IEnumerable<ProjectNuspec> SpecsFor(Solution solution)
        {
            if (!UpdateDependenciesFlag)
            {
                yield break;
            }

            foreach (var project in solution.Projects)
            {
                foreach (var spec in solution.Specifications)
                {
                    if (spec.Name.EqualsIgnoreCase(project.Name))
                    {
                        yield return new ProjectNuspec(project, spec);
                    }
                }
            }
        }
    }

    [CommandDescription("Creates the nuget files locally", Name = "local-nuget")]
    public class LocalNugetCommand : FubuCommand<LocalNugetInput>
    {
        public override bool Execute(LocalNugetInput input)
        {
            return RippleOperation
                .For(input)
                .Step<UpdateNuspecs>()
                .Step<CreatePackages>()
                .Execute();
        }
    }
}