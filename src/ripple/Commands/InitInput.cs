using System;
using System.ComponentModel;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Model;

namespace ripple.Commands
{
    public class InitInput : RippleInput
    {
        [Description("Shorthand name for the project")]
        public string Name { get; set; }
        
        [Description("Relative path to the solution directory.  Default is 'src'")]
        [FlagAlias("src", 's')]
        public string SourceFolderFlag { get; set; }

        [Description("Relative path to the nuspec diretory. Default is 'packaging/nuget'")]
        [FlagAlias("nuspecs", 'n')]
        public string NuspecFolderFlag { get; set; }

        [Description("NuGet feed urls separated by '#'. Defaults to Fubu and NuGet V2 feeds.")]
        public string FeedsFlag { get; set; }

        [Description("Build command for your solution. Defaults to 'rake'")]
        [FlagAlias("build", 'b')]
        public string BuildCommandFlag { get; set; }

        [Description("Fast build command (e.g., rake compile vs. rake). Defaults to 'rake compile'")]
        [FlagAlias("fast-build", 'q')]
        public string FastBuildCommandFlag { get; set; }

        public Solution ToSolution()
        {
            var solution = new Solution {Name = Name};

            SourceFolderFlag.IfNotEmpty(x => solution.SourceFolder = x);
            NuspecFolderFlag.IfNotEmpty(x => solution.NugetSpecFolder = x);

            FeedsFlag.IfNotEmpty(x =>
            {
                solution.ClearFeeds();
                solution.AddFeeds(x.GetFeeds());
            });

            return solution;
        }
    }

    public static class StringExtensions
    {
        public static void IfNotEmpty(this string value, Action<string> continuation)
        {
            if (value.IsNotEmpty())
            {
                continuation(value);
            }
        }

		public static string AppendRandomPath(this string path)
		{
			return path.AppendPath((Guid.NewGuid().ToString().Replace("-", String.Empty)));
		}
    }
}