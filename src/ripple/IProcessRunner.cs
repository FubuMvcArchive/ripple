using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using FubuCore;

namespace ripple
{
    public interface IProcessRunner
    {
        ProcessReturn Run(ProcessStartInfo info, TimeSpan waitDuration, Action<string> callback);
        ProcessReturn Run(ProcessStartInfo info, Action<string> callback);
    }

    // TODO -- move to FubuCore
    public class ProcessRunner : IProcessRunner
    {
        public ProcessReturn Run(ProcessStartInfo info, TimeSpan waitDuration, Action<string> callback)
        {
            //use the operating system shell to start the process
            //this allows credentials to flow through.
            //info.UseShellExecute = true; 
            info.UseShellExecute = false;
            info.Verb = "runas";
            info.WindowStyle = ProcessWindowStyle.Normal;

            //don't open a new terminal window
            info.CreateNoWindow = false;

            info.RedirectStandardError = info.RedirectStandardOutput = true;

            //if (!Path.IsPathRooted(info.FileName))
            //{
            //    info.FileName = info.WorkingDirectory.AppendPath(info.FileName);
            //}

            ProcessReturn returnValue = null;
            var output = new StringBuilder();
            int pid = 0;
            using (var proc = Process.Start(info))
            {
                pid = proc.Id;
                proc.OutputDataReceived += (sender, outputLine) =>
                {
                    if (outputLine.Data.IsNotEmpty())
                    {
                        callback(outputLine.Data);
                    }
                    output.AppendLine(outputLine.Data);
                };

                proc.BeginOutputReadLine();
                proc.WaitForExit((int)waitDuration.TotalMilliseconds);

                killProcessIfItStillExists(pid);

                returnValue = new ProcessReturn()
                {
                    ExitCode = proc.ExitCode,
                    OutputText = output.ToString()
                };
            }

            return returnValue;
        }

        private void killProcessIfItStillExists(int pid)
        {
            if (Process.GetProcesses()
                .Where(p => p.Id == pid)
                .Any())
            {
                try
                {
                    var p = Process.GetProcessById(pid);
                    if (!p.HasExited)
                    {
                        p.Kill();
                        Thread.Sleep(100);
                    }
                }
                catch (ArgumentException)
                {
                    //ignore
                }
            }
        }

        public ProcessReturn Run(ProcessStartInfo info, Action<string> callback)
        {
            return Run(info, new TimeSpan(0, 0, 0, 10), callback);
        }
    }
}