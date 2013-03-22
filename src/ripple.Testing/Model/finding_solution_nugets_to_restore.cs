using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Model
{
	[TestFixture]
	public class finding_solution_nugets_to_restore
	{
		private Solution theSolution;
		private IFeedProvider theFeedProvider;
		private IFloatingFeed theFubuFeed;
		private INugetFeed theNugetFeed;

		private Dependency fubucore;
		private Dependency bottles;
		private Dependency rhinomocks;
		private Dependency structuremap;

		[SetUp]
		public void SetUp()
		{
			theSolution = new Solution();
			theSolution.ClearFeeds();

			theSolution.AddFeed(Feed.Fubu);
			theSolution.AddFeed(Feed.NuGetV2);

			theSolution.AddDependency(bottles = new Dependency("Bottles"));
			theSolution.AddDependency(fubucore = new Dependency("FubuCore", "1.0.1.201"));
			theSolution.AddDependency(rhinomocks = new Dependency("RhinoMocks", "3.6.1", UpdateMode.Fixed));
			theSolution.AddDependency(structuremap = new Dependency("StructureMap", "2.6.3", UpdateMode.Fixed));

			theFubuFeed = MockRepository.GenerateStub<IFloatingFeed>();
			theFubuFeed.Stub(x => x.GetLatest()).Return(new IRemoteNuget[]
			{
				new StubNuget("Bottles", "1.0.2.2"),
				new StubNuget("FubuCore", "1.0.2.232"), 
				new StubNuget("StructureMap", "2.6.4.71"),
			});

			theNugetFeed = MockRepository.GenerateStub<INugetFeed>();
			theNugetFeed.Stub(x => x.Find(rhinomocks)).Return(new StubNuget("RhinoMocks", "3.6.1"));
			theNugetFeed.Stub(x => x.Find(structuremap)).Return(new StubNuget("StructureMap", "2.6.3"));

			theFeedProvider = MockRepository.GenerateStub<IFeedProvider>();
			theFeedProvider.Stub(x => x.For(Feed.Fubu)).Return(theFubuFeed);
			theFeedProvider.Stub(x => x.For(Feed.NuGetV2)).Return(theNugetFeed);

			FeedRegistry.Stub(theFeedProvider);
		}

		private void theVersionIs(Dependency dependency, string version)
		{
			theSolution.Restore(dependency).Version.ToString().ShouldEqual(version);
		}

		[Test]
		public void finds_floated_nuget_without_a_version()
		{
			theVersionIs(bottles, "1.0.2.2");
		}

		[Test]
		public void finds_latest_version_from_floated_feed()
		{
			theVersionIs(fubucore, "1.0.2.232");
		}

		[Test]
		public void restores_the_fixed_version()
		{
			theVersionIs(rhinomocks, "3.6.1.0");
			theVersionIs(structuremap, "2.6.3");
		}
	}

	[TestFixture]
	public class finding_project_nugets_to_restore
	{
		private Solution theSolution;
		private IFeedProvider theFeedProvider;
		private IFloatingFeed theFubuFeed;
		private INugetFeed theNugetFeed;

		private Dependency fubucore;
		private Dependency bottles;
		private Dependency rhinomocks;
		private Dependency structuremap;

		[SetUp]
		public void SetUp()
		{
			theSolution = new Solution();
			theSolution.ClearFeeds();

			theSolution.AddFeed(Feed.Fubu);
			theSolution.AddFeed(Feed.NuGetV2);

			theSolution.AddDependency(fubucore = new Dependency("FubuCore", "1.0.1.201"));

			var theProject = new Project("Test.csproj");
			theSolution.AddProject(theProject);

			theProject.AddDependency(bottles = new Dependency("Bottles"));
			theProject.AddDependency(rhinomocks = new Dependency("RhinoMocks", "3.6.1", UpdateMode.Fixed));
			theProject.AddDependency(structuremap = new Dependency("StructureMap", "2.6.3", UpdateMode.Fixed));

			theFubuFeed = MockRepository.GenerateStub<IFloatingFeed>();
			theFubuFeed.Stub(x => x.GetLatest()).Return(new IRemoteNuget[]
			{
				new StubNuget("Bottles", "1.0.2.2"),
				new StubNuget("FubuCore", "1.0.2.232"), 
				new StubNuget("StructureMap", "2.6.4.71"),
			});

			theNugetFeed = MockRepository.GenerateStub<INugetFeed>();
			theNugetFeed.Stub(x => x.Find(rhinomocks)).Return(new StubNuget("RhinoMocks", "3.6.1"));
			theNugetFeed.Stub(x => x.Find(structuremap)).Return(new StubNuget("StructureMap", "2.6.3"));

			theFeedProvider = MockRepository.GenerateStub<IFeedProvider>();
			theFeedProvider.Stub(x => x.For(Feed.Fubu)).Return(theFubuFeed);
			theFeedProvider.Stub(x => x.For(Feed.NuGetV2)).Return(theNugetFeed);

			FeedRegistry.Stub(theFeedProvider);
		}

		private void theVersionIs(Dependency dependency, string version)
		{
			theSolution.Restore(dependency).Version.ToString().ShouldEqual(version);
		}

		[Test]
		public void finds_floated_nuget_without_a_version()
		{
			theVersionIs(bottles, "1.0.2.2");
		}

		[Test]
		public void finds_latest_version_from_floated_feed()
		{
			theVersionIs(fubucore, "1.0.2.232");
		}

		[Test]
		public void restores_the_fixed_version()
		{
			theVersionIs(rhinomocks, "3.6.1.0");
			theVersionIs(structuremap, "2.6.3");
		}
	}
}