using System;
using System.ComponentModel;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Model;

namespace ripple.Commands
{
    public class OpenNugetInput
    {
        [Description("The Id of the nuget file to open in an editor")]
        public string Name { get; set; }       
    }

    [CommandDescription("Opens your text editor for a given nuspec file by Id, not file name", Name = "open-nuget")]
    public class OpenNugetCommand : FubuCommand<OpenNugetInput>
    {
        public override bool Execute(OpenNugetInput input)
        {
            var solution = SolutionBuilder.ReadFromCurrentDirectory();
            var nuspec = solution.Specifications.FirstOrDefault(x => x.Name.EqualsIgnoreCase(input.Name));

            if (nuspec != null)
            {
                new FileSystem().LaunchEditor(nuspec.Filename);
            }

            return true;
        }
    }
}