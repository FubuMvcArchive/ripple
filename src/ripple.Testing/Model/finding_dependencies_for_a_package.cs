using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using ripple.Model;

namespace ripple.Testing.Model
{
	[TestFixture]
	public class finding_dependencies_for_a_package : InteractionContext<FeedService> // just for the partial mock
	{
		private StubPackageRepository theFubuRepository;
		private StubPackageRepository theNugetRepository;
		private Feed theCustomFeed;
		private Solution theSolution;

		private Dependency fubuJson;

		protected override void beforeEach()
		{
			theCustomFeed = new Feed("/custom");
			theFubuRepository = new StubPackageRepository();
			theNugetRepository = new StubPackageRepository();
			
			theSolution = Solution.Empty();
			theSolution.AddFeed(theCustomFeed);
			theSolution.AddFeed(Feed.Fubu);
			theSolution.AddFeed(Feed.NuGetV2);

			theFubuRepository.ConfigurePackage("FubuJson", "0.1.3.1", package =>
			{
				package.DependsOn("Newtonsoft.Json", "4.5.9");
			});

			// FeedService should use the first package it finds (in the order the Feeds are specified)
			theNugetRepository.ConfigurePackage("FubuJson", "0.1.1.1", package =>
			{
				package.DependsOn("Newtonsoft.Json", "4.1.1");
			});

			FeedScenario.Create(scenario =>
			{
				scenario.For(Feed.Fubu)
					.Add("FubuJson", "0.1.3.1")
					.Add("Newtonsoft.Json", "4.5.9")
					.UseRepository(theFubuRepository);

				scenario.For(Feed.NuGetV2)
					.Add("FubuJson", "0.1.1.1")
					.UseRepository(theNugetRepository);
			});

			fubuJson = new Dependency("FubuJson");

			Services.PartialMockTheClassUnderTest();
			ClassUnderTest.Stub(x => x.NugetFor(theSolution, fubuJson)).Return(new StubNuget("FubuJson", "0.1.3.1"));
		}

		[Test]
		public void lists_dependencies_from_the_first_feed_found()
		{
			var newtonsoft = ClassUnderTest.DependenciesFor(theSolution, fubuJson, UpdateMode.Fixed).Single();
			newtonsoft.Name.ShouldEqual("Newtonsoft.Json");
			newtonsoft.Version.ShouldEqual("4.5.9");
		}
	}

	[TestFixture]
	public class finding_dependencies_for_a_package_with_dependencies : InteractionContext<FeedService> // just for the partial mock
	{
		private StubPackageRepository theFubuRepository;
		private StubPackageRepository theNugetRepository;
		private Feed theCustomFeed;
		private Solution theSolution;

		private Dependency theDependency;

		protected override void beforeEach()
		{
			theCustomFeed = new Feed("/custom");
			theFubuRepository = new StubPackageRepository();
			theNugetRepository = new StubPackageRepository();

			theSolution = Solution.Empty();
			theSolution.AddFeed(theCustomFeed);
			theSolution.AddFeed(Feed.Fubu);
			theSolution.AddFeed(Feed.NuGetV2);

			theFubuRepository.ConfigurePackage("FubuMVC.Json", "0.1.3.1", package =>
			{
				package.DependsOn("FubuJson", "0.1.3.1");
			});

			theFubuRepository.ConfigurePackage("FubuJson", "0.1.3.1", package =>
			{
				package.DependsOn("Newtonsoft.Json", "4.5.9");
			});

			theNugetRepository.AddPackage(new StubPackage("Newtonsoft.Json", "4.5.9"));

			FeedScenario.Create(scenario =>
			{
				scenario.For(Feed.Fubu)
					.Add("FubuMVC.Json", "0.1.3.1")
					.Add("FubuJson", "0.1.3.1")
					.UseRepository(theFubuRepository);

				scenario.For(Feed.NuGetV2)
					.Add("Newtonsoft.Json", "4.5.9")
					.UseRepository(theNugetRepository);
			});

			theDependency = new Dependency("FubuMVC.Json");

			Services.PartialMockTheClassUnderTest();
			ClassUnderTest.Stub(x => x.NugetFor(theSolution, theDependency)).Return(new StubNuget("FubuMVC.Json", "0.1.3.1"));
		}

		[Test]
		public void walks_the_dependencies()
		{
			var dependencies = ClassUnderTest.DependenciesFor(theSolution, theDependency, UpdateMode.Fixed);
			
			dependencies.ShouldHaveTheSameElementKeysAs(new [] { "FubuJson", "Newtonsoft.Json" }, x => x.Name);
		}
	}
}