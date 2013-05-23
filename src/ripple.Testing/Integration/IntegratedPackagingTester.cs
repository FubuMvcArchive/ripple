using System.Linq;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using NuGet;
using ripple.Model;

namespace ripple.Testing.Integration
{
	[TestFixture]
	public class IntegratedPackagingTester
	{
		private SolutionGraphScenario theScenario;

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

			theNugetDirectory = theScenario.Directory.AppendPath("nugets");

			theFileSystem = new FileSystem();
			theFileSystem.CreateDirectory(theNugetDirectory);

			theSolution = theScenario.Find("FubuCore");

			theSolution.Package(theSolution.Specifications.Single(), new SemanticVersion("1.1.1.1"), theNugetDirectory, false);
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