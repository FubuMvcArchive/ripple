using System;
using FubuCore;
using ripple.New.Model;
using ripple.Testing.New.Model;

namespace ripple.Testing.New
{
	public class FeedScenario
	{
		private readonly StubFeedProvider theProvider = new StubFeedProvider();

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