using System;
using System.Collections.Generic;
using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using NuGet;
using ripple.Commands;
using ripple.Model;

namespace ripple.Publishing
{
    public class PublishInput : SolutionInput
    {
        public PublishInput()
        {
            ArtifactsFlag = "artifacts";

            ApiKey = Environment.GetEnvironmentVariable(PublishingService.ApiKey, EnvironmentVariableTarget.User);
            ServerFlag = "https://nuget.org/";
        }

        [Description("Nuget version number")]
        public string Version { get; set; }

        [Description("Api key for the NuGet server")]
        public string ApiKey { get; set; }

        [Description("Overrides the location of the artifacts folder")]
        [FlagAlias("artifacts", 'r')]
        public string ArtifactsFlag { get; set; }

        [Description("Custom url for the NuGet server")]
        [FlagAlias("server", 's')]
        public string ServerFlag { get; set; }

        [Description("Also create symbols packages")]
        [FlagAlias("symbols")]
        public bool CreateSymbolsFlag { get; set; }
    }
    
    [CommandDescription("Builds and pushes all the nuspec files for a solution(s) to nuget.org")]
    public class PublishCommand : FubuCommand<PublishInput>
    {
        public override bool Execute(PublishInput input)
        {
            var report = new PublishReport();

            input.EachSolution(solution =>
            {
                RippleLog.Info("Building nuget files for " + solution.Name);
                var artifactDirectory = solution.Directory.AppendPath(input.ArtifactsFlag);

                RippleLog.Info("Cleaning out any existing nuget files before continuing");
                new FileSystem().CleanDirectory(artifactDirectory);
                
                solution.Specifications.Each(nuget =>
                {
                    RippleLog.Info("Creating and publishing Nuget for " + nuget.Name);

                    var packageFile = solution.Package(new PackageParams(nuget, SemanticVersion.Parse(input.Version), artifactDirectory,  input.CreateSymbolsFlag));
                    var detail = solution.Publisher.PublishPackage(input.ServerFlag, packageFile, input.ApiKey);

                    report.Add(detail);
                });
            });

            RippleLog.InfoMessage(report);

            if (!report.IsSuccessful())
            {
                RippleAssert.Fail("Failure publishing packages");
            }

            return true;
        }
    }
}