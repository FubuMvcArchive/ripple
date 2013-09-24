using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
{
	[TestFixture]
	public class SolutionFilesIntegratedTester
	{
		private SolutionGraphScenario theScenario;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			theScenario = SolutionGraphScenario.Create(scenario =>
			{
				scenario.Solution("FubuCore", fubucore => fubucore.Publishes("FubuCore"));

				scenario.Solution("HtmlTags", htmlTags =>
				{
					htmlTags.Publishes("HtmlTags", x => x.Assembly("HtmlTags.dll", "lib\\4.0"));
					htmlTags.Mode(SolutionMode.NuGet);
				});
			});
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			theScenario.Cleanup();
		}

		[Test]
		public void files_for_ripple_solution()
		{
			SolutionFiles
				.FromDirectory(theScenario.DirectoryForSolution("FubuCore"))
				.Mode
				.ShouldEqual(SolutionMode.Ripple);
		}

		[Test]
		public void files_for_classic_solution()
		{
			SolutionFiles
				.FromDirectory(theScenario.DirectoryForSolution("HtmlTags"))
				.Mode
				.ShouldEqual(SolutionMode.NuGet);
		}
	}
}