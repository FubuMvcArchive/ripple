using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Commands;
using ripple.Model;
using ripple.Steps;

namespace ripple.Testing.Integration
{
	[TestFixture]
	public class IntegratedRestoreWithError
	{
		private SolutionScenario theScenario;
		private Solution theSolution;


		[SetUp]
		public void SetUp()
		{
			theScenario = SolutionScenario.Create(scenario =>
			{
				scenario.Solution("FubuMVC", fubumvc =>
				{
					fubumvc.ProjectDependency("FubuMVC.Core", "FubuCore");
					fubumvc.ProjectDependency("FubuMVC.Core", "Bottles");
				});
			});

			theSolution = theScenario.Find("FubuMVC");
			theSolution.ClearFeeds();
			theSolution.AddFeed(new Feed("http://local"));

			FeedScenario.Create(scenario =>
			{
				scenario.For("http://local")
					.Add("FubuCore", "1.0.0.1")
					.Add("Bottles", "1.0.0.0");
			});
		}

		[TearDown]
		public void TearDown()
		{
			theScenario.Cleanup();
		}

		[Test]
		public void cleans_up_the_orphaned_nupkg_files()
		{
			RippleOperation
				.For<SolutionInput>(new SolutionInput(), theSolution)
				.Step<DownloadMissingNugets>()
				.Step<ExplodeDownloadedNugets>()
				.Step<FixReferences>()
				.Execute();

			var fileSystem = new FileSystem();
			var files = fileSystem.FindFiles(theSolution.PackagesDirectory(), new FileSet {DeepSearch = false, Include = "*.nupkg"});

			files.ShouldHaveCount(0);
		}
	}
}