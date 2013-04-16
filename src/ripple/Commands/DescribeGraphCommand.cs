using System;
using FubuCore.CommandLine;
using FubuCore.Descriptions;
using ripple.Model;

namespace ripple.Commands
{
    public class DescribeGraphInput { }

    [CommandDescription("Describes the current solution graph", Name = "describe-graph")]
    public class DescribeGraphCommand : FubuCommand<DescribeGraphInput>
    {
        public override bool Execute(DescribeGraphInput input)
        {
            var graph = SolutionGraphBuilder.BuildForCurrentDirectory();
            Console.WriteLine(graph.ToDescriptionText());

            return true;
        }
    }
}