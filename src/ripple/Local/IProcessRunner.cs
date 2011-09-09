using System;
using System.Diagnostics;

namespace ripple.Local
{
    public interface IProcessRunner
    {
        ProcessReturn Run(ProcessStartInfo info, TimeSpan waitDuration);
        ProcessReturn Run(ProcessStartInfo info);
    }
}