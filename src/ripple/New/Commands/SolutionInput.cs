using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using ripple.New.Model;
using ripple.New.Nuget;

namespace ripple.New.Commands
{
	public class SolutionInput
	{
		[Description("override the solution to be cleaned")]
		[FlagAlias("solution", 'l')]
		public string SolutionFlag { get; set; }

		[Description("Override the NuGet cache")]
		[FlagAlias("cache", 'c')]
		public string CacheFlag { get; set; }

		public virtual void Apply(Solution solution)
		{
			if (CacheFlag.IsNotEmpty())
			{
				solution.UseCache(new NugetFolderCache(CacheFlag.ToFullPath()));
			}
		}

		public virtual string DescribePlan(Solution solution)
		{
			return string.Empty;
		}
	}
}