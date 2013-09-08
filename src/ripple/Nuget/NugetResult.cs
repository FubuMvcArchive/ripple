using System;
using System.Collections.Generic;

namespace ripple.Nuget
{
    public class NugetResult
    {
        private readonly IList<NugetProblem> _problems = new List<NugetProblem>();

        public bool Found { get { return Nuget != null; } }
        public IRemoteNuget Nuget { get; set; }
        public IEnumerable<NugetProblem> Problems { get { return _problems; } }

        public void AddProblem(Exception exception)
        {
            AddProblem(new NugetProblem(exception.Message, exception));
        }

        public NugetProblem AddProblem(AggregateException exception)
        {
            var message = "Fatal error";
            if (exception != null)
            {
                exception = exception.Flatten();
                message = exception.Message;

                if (exception.InnerException != null)
                {
                    message = exception.InnerException.Message;
                }
            }

            var problem = new NugetProblem(message, exception);
            AddProblem(problem);

            return problem;
        }

        public void AddProblem(NugetProblem problem)
        {
            _problems.Add(problem);
        }

        public void Import(NugetResult result)
        {
            Nuget = result.Nuget;
            _problems.AddRange(result._problems);
        }

        public static NugetResult For(IRemoteNuget nuget)
        {
            return new NugetResult {Nuget = nuget};
        }
    }
}