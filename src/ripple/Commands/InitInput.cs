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

        [FlagAlias("src")]
        [Description("Relative path to the solution directory.  Default is 'src'")]
        public string SourceFolderFlag { get; set; }

        [Description("Open the ripple.config file after creating it")]
        public bool OpenFlag { get; set; }

        public SolutionConfig BuildConfig()
        {
            var config = new SolutionConfig()
                         {
                             Name = Name
                         };

            if (SourceFolderFlag.IsNotEmpty())
            {
                config.SourceFolder = SourceFolderFlag;
            }

            return config;
        }

    }
}