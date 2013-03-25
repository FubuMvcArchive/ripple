using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
{
	[TestFixture]
	public class FeedTester
	{
		[Test]
		public void finds_the_existing_feed()
		{
			var feed = Feed.FindOrCreate(Feed.Fubu.Url);

			feed.ShouldEqual(Feed.Fubu);
			feed.Mode.ShouldEqual(UpdateMode.Float);
		}

		[Test]
		public void creates_feed_if_not_found()
		{
			var feed = Feed.FindOrCreate("http://test");
			feed.Url.ShouldEqual("http://test");
			feed.Mode.ShouldEqual(UpdateMode.Fixed);
		}
	}
}