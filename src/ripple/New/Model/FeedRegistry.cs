using ripple.New.Nuget;

namespace ripple.New.Model
{
	public interface IFeedProvider
	{
		INugetFeed For(Feed feed);
	}

	public class FeedProvider : IFeedProvider
	{
		public INugetFeed For(Feed feed)
		{
			if (feed.Mode == UpdateMode.Fixed)
			{
				return new NugetFeed(feed.Url);
			}

			return new FloatingFeed(feed.Url);
		}
	}

	public class FeedRegistry
	{
		private static IFeedProvider _provider;

		static FeedRegistry()
		{
			Stub(new FeedProvider());
		}

		public static void Stub(IFeedProvider provider)
		{
			_provider = provider;
		}

	 	public static INugetFeed FeedFor(Feed feed)
	 	{
	 		return _provider.For(feed);
	 	}
	}
}