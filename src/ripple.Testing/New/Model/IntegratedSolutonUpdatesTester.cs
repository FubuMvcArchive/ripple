using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.New.Model;

namespace ripple.Testing.New.Model
{
	[TestFixture]
	public class IntegratedSolutonUpdatesTester
	{
		private Solution theSolution;
		private StubNugetStorage theStorage;

		private StubFeed theFeed;

		[SetUp]
		public void SetUp()
		{
			theStorage = new StubNugetStorage();
			theStorage.Add("FubuCore", "1.0.0.0");
			theStorage.Add("Bottles", "1.0.0.0");
			theStorage.Add("FubuLocalization", "1.0.0.0");

			theSolution = new Solution();
			theSolution.AddDependency(new Dependency("FubuCore", "1.0.0.0"));
			theSolution.AddDependency(new Dependency("Bottles", "1.0.0.0"));
			theSolution.AddDependency(new Dependency("FubuLocalization", "1.0.0.0"));
			theSolution.AddFeed(new Feed("local"));
			theSolution.UseStorage(theStorage);

			theFeed = new StubFeed();
			theFeed.Add("FubuCore", "1.0.0.1");
			theFeed.Add("Bottles", "1.0.0.0");

			FeedRegistry.Stub(new StubFeedProvider(theFeed));
		}

		[Test]
		public void finds_the_update()
		{
			var updates = theSolution.Updates();
			updates.ShouldHaveCount(1);
			
			var fubucore = updates.Single();
			fubucore.Name.ShouldEqual("FubuCore");
			fubucore.Version.ToString().ShouldEqual("1.0.0.1");
		}
	}
}