using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Commands;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Packaging
{
    public class CreatePackagesInput : SolutionInput
    {
        public CreatePackagesInput()
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

        [Description("Also create symbols packages")]
        [FlagAlias("symbols", 'C')]
        public bool CreateSymbolsFlag { get; set; }

        public IEnumerable<SpecGroup> SpecsFor(Solution solution)
        {
            if (!UpdateDependenciesFlag)
            {
                return new SpecGroup[0];
            }

            var specs = new List<ProjectNuspec>();
            solution.EachProject(project =>
            {
                solution.Specifications.Each(spec =>
                {
                    if (spec.Name.EqualsIgnoreCase(project.Name))
                    {
                        specs.Add(new ProjectNuspec(project, spec));
                    }
                });
            });

            specs.AddRange(solution.Nuspecs.Select(x => x.ToSpec(solution)));

            return specs
                .GroupBy(x => x.Spec)
                .Select(x => new SpecGroup(x.Key, x.Select(y => y.Project)));
        }
    }
}