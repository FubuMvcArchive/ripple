using System;
using FubuCore.CommandLine;
using FubuCore.Descriptions;
using ripple.New.Model;

namespace ripple.New.Commands
{
	public class DescribeInput
	{
		
	}

	public class DescribeCommand : FubuCommand<DescribeInput>
	{
		public override bool Execute(DescribeInput input)
		{
			// TODO -- Create the builder API or bootstrapping mechanism
			// Maybe mark the inputs w/ interface and delegate to a Load method on Repository


			var files = new RepositoryFiles();
			var builder = new RepositoryBuilder(files, ProjectReader.Basic());

			var solution = builder.Build();
			Console.WriteLine(solution.ToDescriptionText());

			return true;
		}
	}
}