using System;

namespace ripple.Model
{
    public class InvalidSyntaxException : Exception
    {
        public InvalidSyntaxException(string message)
            : base(message)
        {
        }
    }
}