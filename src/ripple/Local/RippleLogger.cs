using System;
using System.IO;
using FubuCore;

namespace ripple.Local
{
    public class RippleLogger : IRippleLogger
    {
        private readonly StringWriter _writer = new StringWriter();
        private int _indentionLevel;

        public void Indent(Action action)
        {
            _indentionLevel++;

            try
            {
                action();
            }
            finally
            {
                _indentionLevel--;
            }
        }

        public void Trace(string format, params object[] parameters)
        {
            var text = "".PadRight(_indentionLevel*4) + format.ToFormat(parameters);
            _writer.WriteLine(text);
            Console.WriteLine(text);
        }

        public void WriteTo(string filename)
        {
            new FileSystem().WriteStringToFile(filename, _writer.ToString());
        }
    }
}