using Bottles.Services.Messaging;

namespace ripple
{
    public class RippleApplication
    {
        public static void Start()
        {
            EventAggregator.Start(new RemoteListener(new MessagingHub()));
        }

        public static void Stop()
        {
            EventAggregator.Stop();
        }
    }
}