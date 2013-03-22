using System;
using FubuCore.CommandLine;
using System.Collections.Generic;
using System.Linq;

namespace ripple.Commands
{
    public class LowerBoundsInput : SolutionInput
    {
        public string Nuget { get; set; }
    }

    [CommandDescription("Sets the nuget dependency for all published nuspec's to greater or equal than the current version of the named nuget", Name="lower-bounds")]
    public class LowerBoundsCommand : FubuCommand<LowerBoundsInput>
    {
        public override bool Execute(LowerBoundsInput input)
        {
            input.FindSolutions().Each(solution =>
            {
                var nuget = solution.LocalDependencies().Get(input.Nuget);
                var version = nuget.Version;

                solution.Specifications.Where(x => x.DependsOn(input.Nuget)).Each(nuspec =>
                {
                    Console.WriteLine("Setting the dependent version of {0} in {1} to '{2}'", input.Nuget, nuspec.Name, version);
                    var nuspecDocument = nuspec.ToDocument();
                    nuspecDocument.SetVersion(input.Nuget, version.ToString());
                    nuspecDocument.SaveChanges();
                });
            });

            return true;
        }
    }
}