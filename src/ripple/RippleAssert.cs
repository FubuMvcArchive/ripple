using System;
using FubuCore;

namespace ripple
{
    public class RippleAssert
    {
        public static void Fail(string message, params object[] substitutions)
        {
            RippleLog.Error(message.ToFormat(substitutions));
            throw new RippleFatalError(message);
        }
    }

    public class RippleFatalError : Exception
    {
        public RippleFatalError(string message) : base(message) { }
        public RippleFatalError(string message, Exception ex) : base(message, ex) { }
    }
}