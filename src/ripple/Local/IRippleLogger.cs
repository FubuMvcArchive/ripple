using System;

namespace ripple.Local
{
    public interface IRippleLogger
    {
        void Indent(Action action);
        void Trace(string format, params object[] parameters);
        void WriteLogFile(string filename);
    }
}