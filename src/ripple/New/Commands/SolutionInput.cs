using System.ComponentModel;
using FubuCore.CommandLine;
using ripple.New.Model;

namespace ripple.New.Commands
{
	public class SolutionInput
	{
		[Description("override the solution to be cleaned")]
		[FlagAlias("solution", 'l')]
		public string SolutionFlag { get; set; }

		public virtual string DescribePlan(Solution solution)
		{
			return string.Empty;
		}
	}
}