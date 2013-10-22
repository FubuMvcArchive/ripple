using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Model;
using ripple.Nuget;
using ripple.Steps;

namespace ripple.Commands
{
    public class BatchInstallInput : RippleInput, INugetOperationContext
    {
        internal Action After = () => { };

        [Description("Optionally read the batch instruction(s) from a flat file.")]
        [FlagAlias("file", 'f')]
        public string FileFlag { get; set; }

        public IEnumerable<NugetPlanRequest> Requests(Solution solution)
        {
            BatchOperation operation;
            if (FileFlag.IsNotEmpty())
            {
                var file = Path.Combine(RippleFileSystem.CurrentDirectory(), FileFlag);
                RippleLog.Info("Detected {0}. Reading batch instructions.".ToFormat(FileFlag));

                var contents = File.ReadAllText(file);
                operation = BatchOperation.Parse(solution, contents);

                After = () => File.Delete(file);
            }
            else
            {
                operation = readFromCommandLine(solution);
            }


            return operation.Requests;
        }

        private static BatchOperation readFromCommandLine(Solution solution)
        {
            Console.WriteLine("# Batch installation interactive mode");
            Console.WriteLine("# Install multiple nugets to projects using either of the following formats:");
            Console.WriteLine("#    Project: Nuget [, Nuget]");
            Console.WriteLine("#    Nuget: Project [, Project]");
            Console.WriteLine("#");
            Console.WriteLine("# Nuget can be formatted as:");
            Console.WriteLine("#    NugetID");
            Console.WriteLine("#    NugetID/Version");
            Console.WriteLine("#");
            Console.WriteLine("# Enter multiple lines. Type '', 'q', or 'quit' to quit.");

            var operation = new BatchOperation(solution);
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.IsEmpty() || line == "q" || line == "quit")
                {
                    break;
                }

                operation.ReadLine(line);
            }

            return operation;
        }

        public override void ApplyTo(Solution solution)
        {
            solution.RequestSave();
        }
    }

    [CommandDescription("Provides a mechanism for installing multiple nugets to your solution", Name = "batch-install")]
    public class BatchInstallCommand : FubuCommand<BatchInstallInput>
    {
        public BatchInstallCommand()
        {
            Usage("Reads the batch instructions from the specified flat file").Arguments(x => x.FileFlag);
        }

        public override bool Execute(BatchInstallInput input)
        {
            var returnValue = RippleOperation
                .For(input)
                .Step<BatchNugetOperation>()
                .Step<DownloadMissingNugets>()
                .Step<ExplodeDownloadedNugets>()
                .Step<FixReferences>()
                .Execute();

            if (returnValue)
            {
                input.After();
            }

            return returnValue;
        }
    }

    public class BatchNugetOperation : IRippleStep
    {
        public Solution Solution { get; set; }

        public void Execute(RippleInput input, IRippleStepRunner runner)
        {
            var nugetRunner = new NugetStepRunner(Solution);
            var aggregatePlan = PlanFor(input.As<INugetOperationContext>(), Solution);

            if (aggregatePlan.Any())
            {
                RippleLog.InfoMessage(aggregatePlan);
            }

            aggregatePlan.Execute(nugetRunner);
        }

        public static NugetPlan PlanFor(INugetOperationContext context, Solution solution)
        {
            var aggregatePlan = new NugetPlan();

            var requests = context.Requests(solution).ToArray();
            requests.Where(x => x.Dependency.IsFixed()).Each(request =>
            {
                request.Solution = solution;

                var plan = solution.Builder.PlanFor(request);
                aggregatePlan.Import(plan);
            });

            requests.Where(x => x.Dependency.IsFloat()).Each(request =>
            {
                request.Solution = solution;

                var plan = solution.Builder.PlanFor(request);
                aggregatePlan.Import(plan);
            });

            return aggregatePlan;
        }
    }
}