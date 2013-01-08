using System;
using System.ComponentModel;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;
using System.Collections.Generic;
using System.Linq;

namespace ripple.Commands
{
    public class BatchPublishInput
    {
        [Description("Directory holding the nuget package files to be published")]
        public string Directory { get; set; }

        [Description("APIKey for Nuget.org")]
        public string ApiKey { get; set; }
    }

    [CommandDescription("Batch publishes all the nupkg files in a directory to the main nuget feed")]
    public class BatchPublishCommand : FubuCommand<BatchPublishInput>
    {
        private int _index;
        private int _count;

        public override bool Execute(BatchPublishInput input)
        {
            Console.WriteLine("Looking for *.nupkg files in " + input.Directory);
            var files = new FileSystem().FindFiles(input.Directory, new FileSet {Include = "*.nupkg"});
            _count = files.Count();
            _index = 0;

            files.Each(file => {
                _index++;

                publishFile(file, input);
            });

            return true;
        }

        private void publishFile(string file, BatchPublishInput input)
        {
            var filename = Path.GetFileName(file);
            Console.WriteLine("Trying to publish {0}, {1} or {2}", filename, _index, _count);

            var cmd = "push {0} {1}".ToFormat(file, input.ApiKey);
            CLIRunner.RunNuget(cmd);
        }
    }
}