using System;
using System.Runtime.Serialization;
using FubuCore.Descriptions;

namespace ripple.Nuget
{
    public class NugetProblem : DescribesItself
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


        public void Describe(Description description)
        {
            var exception = findException(Exception);
            if (exception != null)
            {
                description.Title = exception.GetType().Name;
            }

            description.ShortDescription = Message;
        }

        private Exception findException(Exception exception)
        {
            if (exception == null) return null;

            if (exception.InnerException == null)
            {
                return exception;
            }

            return findException(exception.InnerException);
        }
        
    }
}