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

		public static void Create(Action<FeedScenario> configure)
		{
			var scenario = new FeedScenario();
			configure(scenario);

			FeedRegistry.Stub(scenario.theProvider);
		}
	}
}