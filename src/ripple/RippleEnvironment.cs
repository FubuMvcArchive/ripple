using System;
using System.Net;
using ripple.Model;

namespace ripple
{
    public class RippleEnvironment
    {
        private static Func<bool> _connected;

        private readonly static Lazy<bool> HasConnection;

        static RippleEnvironment()
        {
            HasConnection = new Lazy<bool>(() =>
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        using (var stream = client.OpenRead(Feed.NuGetV2.Url))
                        {
                            return true;
                        }
                    }
                }
                catch
                {
                    return false;
                }
            });

            Live();
        }

        public static void Live()
        {
            _connected = () => HasConnection.Value;
        }

        public static void StubConnection(bool connected)
        {
            _connected = () => connected;
        }

        public static bool Connected()
        {
            return _connected();
        }
    }
}