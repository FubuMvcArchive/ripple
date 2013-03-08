using StoryTeller;
using StoryTeller.Engine;
using ripple.New.Model;

namespace ripple.StoryTeller.Fixtures
{
	public class SolutionFixture : Fixture
	{
		private Solution _solution;
		private ConfiguredFeedProvider _provider;

		public override void SetUp(ITestContext context)
		{
			_solution = new Solution { Name = "ripple-test" };
			_solution.ClearFeeds();

			_provider = new ConfiguredFeedProvider(new ConfiguredFeedCollection());

			FeedRegistry.Stub(_provider);

			Store(_solution);
			Store(_provider);
		}

		public IGrammar SolutionLevelDependencies()
		{
			return CreateNewObject<Dependency>(x =>
			{
				x.SetProperty(d => d.Name);
				x.SetProperty(d => d.Version, "NULL");

				x.WithInput<string>("Mode").Configure((dependency, input) =>
				{
					var mode = UpdateMode.Float;
					UpdateMode.TryParse(input, out mode);

					dependency.Mode = mode;

				}).DefaultValue = "Float";

				x.Do = dependency => _solution.AddDependency(dependency);

			}).AsTable("The solution-level dependencies are");
		}
		
		[ExposeAsTable("The feeds are")]
		public void TheFeedsAre(string Name, string DependencyId, string DependencyVersion)
		{
			var feed = _provider.For(Name);
			feed.Add(DependencyId, DependencyVersion);
		}

		public IGrammar TheSolutionFeedsAre()
		{
			return CreateNewObject<Feed>(x =>
			{
				x.WithInput<string>("Name").Configure((feed, input) =>
				{
					feed.Url = input;
				});

				x.WithInput<string>("Mode").Configure((feed, input) =>
				{
					var mode = UpdateMode.Float;
					UpdateMode.TryParse(input, out mode);

					feed.Mode = mode;

				}).DefaultValue = "Float";

				x.Do = feed => _solution.AddFeed(feed);

			}).AsTable("The solution feeds are");
		}
	}
}