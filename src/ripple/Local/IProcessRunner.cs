using System;
using System.Diagnostics;

namespace ripple.Local
{
    public interface IProcessRunner
    {
        ProcessReturn Run(ProcessStartInfo info, TimeSpan waitDuration, Action<string> callback);
        ProcessReturn Run(ProcessStartInfo info, Action<string> callback);
    }
}