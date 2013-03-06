using FubuTestingSupport;
using NUnit.Framework;
using ripple.New.Model;

namespace ripple.Testing.New.Model
{
	[TestFixture]
	public class finding_the_latest_dependency
	{
		private FeedService theFeedService;
		private StubFeed theFeed;
		private Solution theSolution;

		[SetUp]
		public void SetUp()
		{
			theFeedService = new FeedService();
			
			theFeed = new StubFeed();
			theFeed.Add(new Dependency("Bottles", "1.0.0.0"));
			theFeed.Add(new Dependency("Bottles", "1.0.1.0"));
			theFeed.Add(new Dependency("StructureMap", "2.6.4.54"));
			FeedRegistry.Stub(new StubFeedProvider(theFeed));

			theSolution = new Solution();
			theSolution.AddFeed(new Feed("testing"));
			theSolution.AddDependency(new Dependency("Bottles", "1.0.0.0"));
			theSolution.AddDependency(new Dependency("StructureMap", "2.6.3", UpdateMode.Fixed));
		}

		[Test]
		public void latest_for_floated_nuget()
		{
			var nuget = theFeedService.LatestFor(theSolution, theSolution.FindDependency("Bottles"));
			nuget.Name.ShouldEqual("Bottles");
			nuget.Version.ToString().ShouldEqual("1.0.1.0");
		}

		[Test]
		public void latest_for_fixed()
		{
			theFeedService.LatestFor(theSolution, theSolution.FindDependency("StructureMap")).ShouldBeNull();
		}

		[Test]
		public void null_if_id_is_not_found()
		{
			theFeedService
				.LatestFor(theSolution, new Dependency("FubuCore"))
				.ShouldBeNull();
		}
	}
}