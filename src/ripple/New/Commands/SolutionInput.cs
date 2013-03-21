using System;
using System.Collections.Generic;
using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Model;
using ripple.New.Model;
using ripple.New.Nuget;

namespace ripple.New.Commands
{
	public class SolutionInput
	{
		private readonly Lazy<SolutionGraph> _graph = new Lazy<SolutionGraph>(SolutionGraphBuilder.BuildForRippleDirectory);

		public SolutionInput()
		{
			ModeFlag = SolutionMode.Ripple;
		}

		[Description("override the solution to be cleaned")]
		[FlagAlias("solution", 'l')]
		public string SolutionFlag { get; set; }

		[Description("Override the Solution mode (Classic or Ripple)")]
		[FlagAlias("mode", 'm')]
		public SolutionMode ModeFlag { get; set; }

		[Description("Override the NuGet cache")]
		[FlagAlias("cache", 'c')]
		public string CacheFlag { get; set; }

		[Description("Apply restore to all solutions")]
		public bool AllFlag { get; set; }

		public void EachSolution(Action<Solution> configure)
		{
			FindSolutions().Each(configure);
		}

		public IEnumerable<Solution> FindSolutions()
		{
			if (SolutionFlag.IsNotEmpty())
			{
				yield return _graph.Value[SolutionFlag];
			}
			else if (AllFlag || ".".ToFullPath() == RippleFileSystem.CodeDirectory())
			{
				foreach (var solution in _graph.Value.AllSolutions)
				{
					yield return solution;
				}
			}
			else
			{
				if (new FileSystem().FileExists(".".ToFullPath(), SolutionFiles.ConfigFile))
				{
					yield return SolutionBuilder.ReadFrom(".");
				}
			}
		}

		public void Apply(Solution solution)
		{
			if (CacheFlag.IsNotEmpty())
			{
				solution.UseCache(new NugetFolderCache(solution, CacheFlag.ToFullPath()));
			}

			ApplyTo(solution);
		}

		public virtual void ApplyTo(Solution solution)
		{
		}

		public virtual string DescribePlan(Solution solution)
		{
			return string.Empty;
		}
	}
}