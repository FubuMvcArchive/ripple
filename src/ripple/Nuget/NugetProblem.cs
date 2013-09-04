using System;

namespace ripple.Nuget
{
    public class NugetProblem
    {
        public NugetProblem(string message)
            : this(message, null)
        {
        }

        public NugetProblem(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }

        public string Message { get; private set; }
        public Exception Exception { get; private set; }
    }
}