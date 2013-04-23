using System;
using System.IO;
using FubuCore.CommandLine;
using FubuCore;
using ripple.Model;
using ripple.Runners;

namespace ripple.Commands
{
    [CommandDescription("Initializes a project directory as a 'rippled' project")]
    public class InitCommand : FubuCommand<InitInput>
    {

        public override bool Execute(InitInput input)
        {
            var config = input.BuildConfig();

            var fileSystem = new FileSystem();

            var rippleFilename = input.GetRippleFileName();

            
            writeRippleCmdFile(rippleFilename, fileSystem);

            writeRippleConfig(rippleFilename, fileSystem, config);
            openRippleConfigFile(input, rippleFilename, fileSystem);

            writeGitIgnore(config);

            removePackagesFromGit(config);

            return true;
        }

        private void writeRippleCmdFile(string rippleFilename, FileSystem fileSystem)
        {
            var cmdFile = rippleFilename.ParentDirectory().AppendPath("ripple.cmd");
            Console.WriteLine("Writing out " + cmdFile);
            fileSystem.WriteStringToFile(cmdFile, @"buildsupport\ripple.exe %*");
        }

        private static void removePackagesFromGit(SolutionConfig config)
        {
            var packagesFolder = FileSystem.Combine(config.SourceFolder, "packages");
            var arguments = "rm {0} -r".ToFormat(packagesFolder);

            Runner.Git.Run(arguments);
            Runner.Git.Run("status");
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

    // TODO -- clean up the ProcessRunner
}