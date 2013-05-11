using System;
using System.ComponentModel;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;
using System.Collections.Generic;
using ripple.Model;
using ripple.Steps;

namespace ripple.Commands
{
    public class HistoryInput : SolutionInput
    {
        public HistoryInput()
        {
            ArtifactsFlag = "artifacts";
        }

        [Description("Override the local folder where the dependency-history.txt file will be written")]
        [FlagAlias("artifacts", 'r')]
        public string ArtifactsFlag { get; set; }
    }

    public class HistoryCommand : FubuCommand<HistoryInput>
    {
        public override bool Execute(HistoryInput input)
        {
            return RippleOperation
                .For<HistoryInput>(input)
                .Step<WriteHistory>()
                .Execute();
        }
    }

    public class WriteHistory : RippleStep<HistoryInput>
    {
        protected override void execute(HistoryInput input, IRippleStepRunner runner)
        {
            var list = new List<string>();
            var local = Solution.LocalDependencies();

            local.All().Each(x => list.Add("{0}/{1}".ToFormat(x.Name, x.Version)));

            var historyFile = Solution.Directory.AppendPath(input.ArtifactsFlag, "dependency-history.txt");
            Console.WriteLine("Writing nuget dependency history to " + historyFile);

            list.Each(x => Console.WriteLine(x));

            var system = new FileSystem();
            if (!system.DirectoryExists(Solution.Directory, input.ArtifactsFlag))
            {
                system.CreateDirectory(Solution.Directory, input.ArtifactsFlag);
            }

            system.AlterFlatFile(historyFile, record =>
            {
                record.Clear();
                record.AddRange(list);
            });
        }
    }
}