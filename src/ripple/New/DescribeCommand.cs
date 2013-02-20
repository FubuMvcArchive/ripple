using System;
using FubuCore.CommandLine;
using FubuCore.Descriptions;

namespace ripple.New
{
	public class DescribeInput
	{
		
	}

	public class DescribeCommand : FubuCommand<DescribeInput>
	{
		public override bool Execute(DescribeInput input)
		{
			// TODO -- Create the builder API or bootstrapping mechanism
			// Maybe mark the inputs w/ interface and delegate to a Load method on Solution


			var files = new SolutionFiles();
			var builder = new SolutionBuilder(files, ProjectReader.Basic());

			var solution = builder.Build();
			Console.WriteLine(solution.ToDescriptionText());

			return true;
		}
	}
}