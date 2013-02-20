using System;
using StoryTeller;
using StoryTeller.Engine;
using ripple.New;

namespace ripple.StoryTeller.Fixtures
{
	public class SystemStateFixture : Fixture
	{
		public const string SolutionName = "ripple-test";

		private Solution _config;

		public SystemStateFixture()
		{
			Title = "The repository";
		}

		public override void SetUp(ITestContext context)
		{
			_config = new Solution { Name = SolutionName };
		}

		public override void TearDown()
		{
			Store(_config);
		}

		public IGrammar TheFeedsAre()
		{
			return CreateNewObject<Feed>(x =>
			{
				x.SetProperty(f => f.Url);
				x.WithInput<string>("Mode").Configure((feed, mode) =>
				{
					feed.Mode = (UpdateMode)Enum.Parse(typeof(UpdateMode), mode, true);

				}).DefaultValue = UpdateMode.Fixed.ToString();

				x.Do = (feed) => _config.AddFeed(feed);

			}).AsTable("The feeds are").Before(() => _config.ClearFeeds());
		}

		[ExposeAsTable("The projects are")]
		public void TheProjectsAre(string FilePath, string Dependency, string DependencyVersion)
		{
			var project = _config.FindProject(Name);
			if (project == null)
			{
				project = new Project(FilePath) { Solution = _config};
				_config.AddProject(project);
			}

			project.AddDependency(new NugetDependency(Dependency, DependencyVersion));
		}
	}
}