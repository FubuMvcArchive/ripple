using System;
using System.Net;

namespace ripple
{
    public class RippleConnection
    {
        private static Func<bool> _connected;

        static RippleConnection()
        {
            Live();
        }

        public static void Live()
        {
            _connected = () =>
            {
                try
                {
                    using (var client = new WebClient())
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            };
        }

        public static void Stub(bool connected)
        {
            _connected = () => connected;
        }

        public static bool Connected()
        {
            return _connected();
        }
    }
}