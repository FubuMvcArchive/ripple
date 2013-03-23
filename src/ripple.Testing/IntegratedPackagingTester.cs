using System.Linq;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;

namespace ripple.Testing
{
	[TestFixture]
	public class IntegratedPackagingTester
	{
		private SolutionGraphScenario theScenario;
		private SolutionGraphBuilder theBuilder;
		private SolutionGraph theGraph;

		private FileSystem theFileSystem;
		private string theNugetDirectory;

		private Solution theSolution;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			theScenario = SolutionGraphScenario.Create(scenario =>
			{
				scenario.Solution("FubuCore", fubucore => fubucore.Publishes("FubuCore"));
			});

			theBuilder = new SolutionGraphBuilder(new FileSystem());

			theGraph = theBuilder.ReadFrom(theScenario.Directory);

			theNugetDirectory = theScenario.Directory.AppendPath("nugets");

			theFileSystem = new FileSystem();
			theFileSystem.CreateDirectory(theNugetDirectory);

			theSolution = theGraph["FubuCore"];

			theSolution.Package(theSolution.Specifications.Single(), new SemanticVersion("1.1.1.1"), theNugetDirectory);
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			theScenario.Cleanup();
		}

		[Test]
		public void creates_the_published_file()
		{
			theFileSystem.FileExists(theNugetDirectory, "FubuCore.1.1.1.1.nupkg").ShouldBeTrue();

		}
	}
}