using System;
using FubuCore;

namespace ripple.Local
{
    public class InvalidSolutionException : Exception
    {
        public InvalidSolutionException(string solutionName)
            : base("Solution {0} does not exist".ToFormat(solutionName))
        {
        }
    }
}