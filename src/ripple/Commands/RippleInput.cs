using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Commands
{
    public class RippleInput
    {
        [Description("Writes all output to the screen")]
        public bool VerboseFlag { get; set; }

        [Description("Override the NuGet cache")]
        [FlagAlias("cache", 'c')]
        public string CacheFlag { get; set; }

        public void Apply(Solution solution)
        {
            RippleLog.Verbose(VerboseFlag);

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

    public interface IAllowExplicitBranch
    {
        string BranchFlag { get; }
    }
}