using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
{
	[TestFixture]
	public class IntegratedSolutionUpdatesTester
	{
		private Solution theSolution;
		private StubNugetStorage theStorage;

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

			FeedScenario.Create(scenario =>
			{
				scenario.For(new Feed("local"))
					.Add("FubuCore", "1.0.0.1")
					.Add("Bottles", "1.0.0.0");
			});
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

	[TestFixture]
	public class finding_a_forced_update_for_a_fixed_nuget
	{
		private Solution theSolution;
		private StubNugetStorage theStorage;

		[SetUp]
		public void SetUp()
		{
			theStorage = new StubNugetStorage();
			theStorage.Add("FubuCore", "1.0.0.0");
			theStorage.Add("Bottles", "1.0.0.0");
			theStorage.Add("StructureMap", "2.6.3");

			theSolution = Solution.Empty();
			theSolution.AddDependency(new Dependency("FubuCore", "1.0.0.0"));
			theSolution.AddDependency(new Dependency("Bottles", "1.0.0.0"));
			theSolution.AddDependency(new Dependency("FubuLocalization", "1.0.0.0"));
			theSolution.AddDependency(new Dependency("StructureMap", "2.6.3", UpdateMode.Fixed));
			theSolution.AddFeed(Feed.Fubu);
			theSolution.UseStorage(theStorage);

			FeedScenario.Create(scenario =>
			{
				scenario
					.For(Feed.Fubu)
					.Add("StructureMap", "2.6.4.54");
			});
		}

		[Test]
		public void finds_the_update()
		{
			var structuremap = theSolution.UpdateFor("StructureMap");

			structuremap.Name.ShouldEqual("StructureMap");
			structuremap.Version.ToString().ShouldEqual("2.6.4.54");
		}
	}
}