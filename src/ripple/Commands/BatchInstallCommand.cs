using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Model;
using ripple.Nuget;
using ripple.Steps;

namespace ripple.Commands
{
	public class BatchInstallInput : RippleInput, INugetOperationContext
	{
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

				File.Delete(file);
			}
			else
			{
				operation = readFromCommandLine(solution);
			}


			return operation.Requests;
		}

		private static BatchOperation readFromCommandLine(Solution solution)
		{
			Console.WriteLine("# Batch installation mode");
			Console.WriteLine("# Install multiple nugets to projects using either of the following formats:");
			Console.WriteLine("#    Project: Nuget [, Nuget]");
			Console.WriteLine("#    Nuget: Project [, Project]");
			Console.WriteLine("#");
			Console.WriteLine("# Nuget can be formatted as:");
			Console.WriteLine("#    NugetID");
			Console.WriteLine("#    NugetID/Version");
			Console.WriteLine("#");
			Console.WriteLine("# Enter multiple lines. Type 'q' or 'quit' to quit.");

			var operation = new BatchOperation(solution);
			string line;
			while ((line = Console.ReadLine()) != null)
			{
				line = line.Trim();
				if (line == "q" || line == "quit")
				{
					break;
				}

				operation.ReadLine(line);
			}

			return operation;
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
			return RippleOperation
				.For(input)
				.Step<NugetOperation>()
				.Step<DownloadMissingNugets>()
				.Step<ExplodeDownloadedNugets>()
				.Execute();
		}
	}
}