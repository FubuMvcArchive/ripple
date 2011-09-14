using System;
using System.Diagnostics;
using FubuCore;
using FubuCore.CommandLine;
using ripple.Local;

namespace ripple.Commands
{
    public static class CLIRunner
    {
        public static void RunGit(string command, params object[] parameters)
        {

            var gitFile = RippleFileSystem.LocationOfRunner("run-git.cmd");

            var processStartInfo = new ProcessStartInfo(){
                FileName = gitFile,
                Arguments = command.ToFormat(parameters)
            };

            runProcess(processStartInfo);
        }

        public static void RunRake(string commandLine)
        {
            var rakeRunnerFile = RippleFileSystem.RakeRunnerFile();

            var processStartInfo = new ProcessStartInfo()
                                   {
                                       FileName = rakeRunnerFile,
                                       Arguments = commandLine
                                   };

            runProcess(processStartInfo);
        }

        public static void RunNuget(string command, params object[] parameters)
        {
            var nugetFile = RippleFileSystem.LocationOfRunner("nuget.exe");


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

            
            var returnValue = runner.Run(processStartInfo, new TimeSpan(0, 1, 0), text => { });
            var color = returnValue.ExitCode == 0 ? ConsoleColor.Gray : ConsoleColor.Red;

            
            Console.ForegroundColor = color;



            

            Console.WriteLine(returnValue.OutputText);
            Console.WriteLine("ExitCode:  " + returnValue.ExitCode);

            ConsoleWriter.PrintHorizontalLine();

            Console.ForegroundColor = start;
        }
    }
}