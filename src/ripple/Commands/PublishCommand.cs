using System;
using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using System.Collections.Generic;

namespace ripple.Commands
{
    public class PublishInput : SolutionInput
    {
        public PublishInput()
        {
            ArtifactsFlag = "artifacts";
        }

        [Description("Nuget version number")]
        public string Version { get; set; }

        [Description("Api key for the nuget server")]
        public string ApiKey { get; set; }
        

        [Description("Overrides the location of the artifacts folder")]
        public string ArtifactsFlag { get; set; }
    }
    
    [CommandDescription("Builds and pushes all the nuspec files for a solution(s) to nuget.org")]
    public class PublishCommand : FubuCommand<PublishInput>
    {
        public override bool Execute(PublishInput input)
        {
            input.FindSolutions().Each(solution =>
            {
                Console.WriteLine("Building nuget files for " + solution.Name);
                var artifactDirectory = solution.Directory.AppendPath(input.ArtifactsFlag);


                solution.PublishedNugets.Each(nuget =>
                {
                    Console.WriteLine("Creating and publishing Nuget for " + nuget.Name);

                    CLIRunner.RunNuget("pack {0} -version {1} -o {2}", nuget.Filename.FileEscape(), input.Version, artifactDirectory.FileEscape());

                    var nupkgFileName = "{0}.{1}.nupkg".ToFormat(nuget.Name, input.Version);

                    CLIRunner.RunNuget("push {0} {1}", artifactDirectory.AppendPath(nupkgFileName).FileEscape(), input.ApiKey);
                });
            });

            return true;
        }

    }
}