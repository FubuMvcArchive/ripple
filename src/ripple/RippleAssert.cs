using System;
using FubuCore;

namespace ripple
{
    public class RippleAssert
    {
        public static void Fail(string message, params object[] substitutions)
        {
            // TODO -- Hate this
            var formattedMessage = message;
            try
            {
                formattedMessage = message.ToFormat(substitutions);
            }
            catch (FormatException)
            {
                // Just swallow it
            }

            RippleLog.Error(formattedMessage);
            throw new RippleFatalError(message);
        }
    }

    public class RippleFatalError : Exception
    {
        public RippleFatalError(string message) : base(message) { }
        public RippleFatalError(string message, Exception ex) : base(message, ex) { }
    }
}