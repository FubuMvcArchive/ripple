using System;
using FubuCore;
using ripple.Model;
using ripple.Testing.Model;

namespace ripple.Testing
{
	public class FeedScenario
	{
		private readonly StubFeedProvider theProvider = new StubFeedProvider();

		public StubFeed For(string name)
		{
			return For(new Feed(name));
		}

		public StubFeed For(Feed feed)
		{
			return theProvider.For(feed).As<StubFeed>();
		}

        public void Offline()
        {
            RippleConnection.Stub(false);
        }

        public void Online()
        {
            RippleConnection.Stub(true);
        }

		public static void Create(Action<FeedScenario> configure)
		{
			var scenario = new FeedScenario();
            scenario.Online();

			configure(scenario);

			FeedRegistry.Stub(scenario.theProvider);
		}
	}
}