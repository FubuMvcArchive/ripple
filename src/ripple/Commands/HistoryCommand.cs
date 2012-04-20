using System;
using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using System.Collections.Generic;

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
            var system = new FileSystem();

            input.FindSolutions().Each(solution =>
            {
                var list = new List<string>();

                solution.GetAllNugetDependencies().Each(x =>
                {
                    list.Add("{0}/{1}".ToFormat(x.Name, x.Version));
                });

                var historyFile = solution.Directory.AppendPath(input.ArtifactsFlag, "dependency-history.txt");
                Console.WriteLine("Writing nuget dependency history to " + historyFile);

                list.Each(x => Console.WriteLine(x));

                system.AlterFlatFile(historyFile, record =>
                {
                    record.Clear();
                    record.AddRange(list);
                });


            });

            return true;
        }
    }
}