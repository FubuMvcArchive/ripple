using System;
using System.ComponentModel;
using FubuCore.CommandLine;
using FubuCore;

namespace ripple
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

    [CommandDescription("Initializes a project directory as a 'rippled' project")]
    public class InitCommand : FubuCommand<InitInput>
    {

        // TODO -- need to do a .gitignore
        public override bool Execute(InitInput input)
        {
            var config = input.BuildConfig();

            var fileSystem = new FileSystem();

            var rippleFilename = input.GetRippleFileName();

            writeRippleConfig(rippleFilename, fileSystem, config);
            openRippleConfigFile(input, rippleFilename, fileSystem);

            return writeGitIgnore(config);
        }

        private static void writeRippleConfig(string rippleFilename, FileSystem fileSystem, SolutionConfig config)
        {
            ConsoleWriter.Write("Writing new ripple.config to " + rippleFilename);
            fileSystem.WriteObjectToFile(rippleFilename, config);
        }

        private static void openRippleConfigFile(InitInput input, string rippleFilename, FileSystem fileSystem)
        {
            if (input.OpenFlag)
            {
                fileSystem.LaunchEditor(rippleFilename);
            }
        }

        private static bool writeGitIgnore(SolutionConfig config)
        {
            return new GitIgnoreCommand().Execute(new GitIgnoreInput(){
                Line = config.SourceFolder + "/packages"
            });
        }
    }
}