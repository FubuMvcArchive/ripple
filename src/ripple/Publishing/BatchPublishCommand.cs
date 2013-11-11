using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;

namespace ripple.Publishing
{
    public class BatchPublishInput
    {
        public BatchPublishInput()
        {
            ApiKeyFlag = Environment.GetEnvironmentVariable(PublishingService.ApiKey, EnvironmentVariableTarget.User);
            ServerFlag = new PublishInput().ServerFlag;
        }

        [Description("Directory holding the nuget package files to be published")]
        public string Directory { get; set; }

        [Description("API Key for Nuget.org")]
        public string ApiKeyFlag { get; set; }

        [Description("Custom url for the NuGet server")]
        [FlagAlias("server", 's')]
        public string ServerFlag { get; set; }
    }

    [CommandDescription("Batch publishes all the nupkg files in a directory to the main nuget feed", Name = "batch-publish")]
    public class BatchPublishCommand : FubuCommand<BatchPublishInput>
    {
        private int _index;
        private int _count;

        public override bool Execute(BatchPublishInput input)
        {
            RippleLog.Info("Looking for *.nupkg files in " + input.Directory);

            var files = new FileSystem()
                .FindFiles(input.Directory, new FileSet { Include = "*.nupkg" })
                .ToArray();

            _index = 0;
            _count = files.Count();

            var publisher = PublishingService.Basic();
            var report = new PublishReport();

            files.Each(file =>
            {
                _index++;
                RippleLog.Info("Trying to publish {0}, {1} or {2}".ToFormat(file, _index, _count));
                
                var detail = publisher.PublishPackage(input.ServerFlag, file, input.ApiKeyFlag);
                report.Add(detail);
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