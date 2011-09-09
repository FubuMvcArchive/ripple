using FubuCore.CommandLine;
using FubuCore;
using ripple.Model;

namespace ripple.Commands
{
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