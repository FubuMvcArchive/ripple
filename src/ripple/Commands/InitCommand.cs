using System;
using System.Diagnostics;
using System.IO;
using FubuCore.CommandLine;
using FubuCore;
using ripple.Local;
using ripple.Model;

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

            writeRippleConfig(rippleFilename, fileSystem, config);
            openRippleConfigFile(input, rippleFilename, fileSystem);

            writeGitIgnore(config);

            removePackagesFromGit(config);

            return true;
        }

        private static void removePackagesFromGit(SolutionConfig config)
        {

            var packagesFolder = FileSystem.Combine(config.SourceFolder, "packages");

            var arguments = "rm {0} -r".ToFormat(packagesFolder);
            CLIRunner.RunGit(arguments);
            CLIRunner.RunGit("status");
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
    public static class CLIRunner
    {
        public static void RunGit(string command, params object[] parameters)
        {
            
            var gitFile = RippleFileSystem.CodeDirectory().AppendPath("ripple", "run-git.cmd");

            var processStartInfo = new ProcessStartInfo(){
                FileName = gitFile,
                Arguments = command.ToFormat(parameters)
            };

            runProcess(processStartInfo);
        }

        public static void RunNuget(string command, params object[] parameters)
        {
            var nugetFile = RippleFileSystem.CodeDirectory().AppendPath("ripple", "nuget.exe");

            var processStartInfo = new ProcessStartInfo()
            {
                FileName = nugetFile,
                Arguments = command.ToFormat(parameters)
            };

            runProcess(processStartInfo);
        }

        private static void runProcess(ProcessStartInfo processStartInfo)
        {
            var runner = new ProcessRunner();

            var start = Console.ForegroundColor;


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            ConsoleWriter.PrintHorizontalLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("{0} {1}", processStartInfo.FileName, processStartInfo.Arguments);
            ConsoleWriter.PrintHorizontalLine();

            
            var returnValue = runner.Run(processStartInfo, new TimeSpan(0, 1, 0));
            var color = returnValue.ExitCode == 0 ? ConsoleColor.Gray : ConsoleColor.Red;

            
            Console.ForegroundColor = color;



            

            Console.WriteLine(returnValue.OutputText);
            Console.WriteLine("ExitCode:  " + returnValue.ExitCode);

            ConsoleWriter.PrintHorizontalLine();

            Console.ForegroundColor = start;
        }
    }
}