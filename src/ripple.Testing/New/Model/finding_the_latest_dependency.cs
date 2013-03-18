using FubuTestingSupport;
using NUnit.Framework;
using ripple.New.Model;

namespace ripple.Testing.New.Model
{
	[TestFixture]
	public class finding_the_latest_dependency
	{
		private FeedService theFeedService;
		private Feed theFeed;
		private Solution theSolution;
		private StubNugetStorage theStorage;

		[SetUp]
		public void SetUp()
		{
			theFeedService = new FeedService();

			theFeed = new Feed("testing");
			theStorage = new StubNugetStorage();

			theSolution = new Solution();
			theSolution.UseStorage(theStorage);
			theSolution.AddFeed(theFeed);
			theSolution.AddDependency(new Dependency("Bottles", "1.0.0.0"));
			theSolution.AddDependency(new Dependency("FubuCore"));
			theSolution.AddDependency(new Dependency("StructureMap", "2.6.3", UpdateMode.Fixed));

			FeedScenario.Create(scenario =>
			{
				scenario.For(theFeed)
					    .Add(new Dependency("Bottles", "1.0.0.0"))
					    .Add(new Dependency("Bottles", "1.0.1.0"))
					    .Add(new Dependency("FubuCore", "1.2.0.0"))
					    .Add(new Dependency("StructureMap", "2.6.4.54"));
			});

			
		}

		[Test]
		public void latest_for_floated_nuget()
		{
			var nuget = theFeedService.LatestFor(theSolution, theSolution.FindDependency("FubuCore"));
			nuget.Name.ShouldEqual("FubuCore");
			nuget.Version.ToString().ShouldEqual("1.2.0.0");
		}

		[Test]
		public void latest_for_floated_nuget_with_local_dependency()
		{
			theStorage.Add("FubuCore", "1.2.0.0");
			theFeedService.LatestFor(theSolution, theSolution.FindDependency("FubuCore")).ShouldBeNull();
		}

		[Test]
		public void latest_for_nuget_from_floated_feed()
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
				.LatestFor(theSolution, new Dependency("FubuLocalization"))
				.ShouldBeNull();
		}
	}
}