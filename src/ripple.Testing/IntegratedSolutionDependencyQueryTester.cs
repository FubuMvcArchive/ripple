using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing
{
    [TestFixture]
    public class IntegratedSolutionDependencyQueryTester
    {
		private SolutionGraphScenario theScenario;

		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			theScenario = SolutionGraphScenario.Create(scenario =>
			{
				scenario.Solution("Bottles", bottles =>
				{
					bottles.Publishes("Bottles", x => x.Assembly("Bottles.dll", "lib").DependsOn("FubuCore"));
					bottles.ProjectDependency("Bottles", "FubuCore");
				});

				// Defaults to "FubuCore.dll" targeting "lib"
				scenario.Solution("FubuCore", fubucore => fubucore.Publishes("FubuCore"));

				scenario.Solution("FubuLocalization", localization =>
				{
					localization.Publishes("FubuLocalization", x => x.Assembly("FubuLocalization.dll", "lib").DependsOn("FubuCore"));
					localization.ProjectDependency("FubuLocalization", "FubuCore");
				});

				scenario.Solution("FubuMVC", fubumvc =>
				{
					fubumvc.Publishes("FubuMVC.Core", x =>
					{
						x.Assembly("FubuMVC.Core.dll", "lib\\net40");
						x.DependsOn("Bottles");
						x.DependsOn("FubuCore");
						x.DependsOn("FubuLocalization");
						x.DependsOn("HtmlTags");
					});

					fubumvc.ProjectDependency("FubuMVC.Core", "Bottles");
					fubumvc.ProjectDependency("FubuMVC.Core", "FubuCore");
					fubumvc.ProjectDependency("FubuMVC.Core", "FubuLocalization");
					fubumvc.ProjectDependency("FubuMVC.Core", "HtmlTags");
				});

				scenario.Solution("FubuMVC.Core.View", views =>
				{
					views.Publishes("FubuMVC.Core.View", x => x.Assembly("FubuMVC.Core.View.dll", "lib\\net40").DependsOn("FubuMVC.Core"));

					views.ProjectDependency("FubuMVC.Core.View", "Bottles");
					views.ProjectDependency("FubuMVC.Core.View", "FubuCore");
					views.ProjectDependency("FubuMVC.Core.View", "FubuLocalization");
					views.ProjectDependency("FubuMVC.Core.View", "FubuMVC.Core");
					views.ProjectDependency("FubuMVC.Core.View", "HtmlTags");
				});

				scenario.Solution("FubuMVC.Core.UI", ui =>
				{
					ui.Publishes("FubuMVC.Core.UI", x => x.Assembly("FubuMVC.Core.UI.dll", "lib\\net40").DependsOn("FubuMVC.Core.View"));

					ui.ProjectDependency("FubuMVC.Core.UI", "Bottles");
					ui.ProjectDependency("FubuMVC.Core.UI", "FubuCore");
					ui.ProjectDependency("FubuMVC.Core.UI", "FubuLocalization");
					ui.ProjectDependency("FubuMVC.Core.UI", "FubuMVC.Core");
					ui.ProjectDependency("FubuMVC.Core.UI", "FubuMVC.Core.View");
					ui.ProjectDependency("FubuMVC.Core.UI", "HtmlTags");
				});

				scenario.Solution("HtmlTags", htmlTags => htmlTags.Publishes("HtmlTags", x => x.Assembly("HtmlTags.dll", "lib\\4.0")));
			});
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			theScenario.Cleanup();
		}

        [Test]
        public void depends_on_positive()
        {
            theScenario.Find("Bottles").DependsOn(theScenario.Find("FubuCore")).ShouldBeTrue();

			theScenario.Find("FubuLocalization").DependsOn(theScenario.Find("FubuCore")).ShouldBeTrue();

			theScenario.Find("FubuMVC").DependsOn(theScenario.Find("Bottles")).ShouldBeTrue();
			theScenario.Find("FubuMVC").DependsOn(theScenario.Find("FubuCore")).ShouldBeTrue();
			theScenario.Find("FubuMVC").DependsOn(theScenario.Find("FubuLocalization")).ShouldBeTrue();
			theScenario.Find("FubuMVC").DependsOn(theScenario.Find("HtmlTags")).ShouldBeTrue();
        }

        [Test]
        public void depends_on_negative()
        {
			theScenario.Find("FubuCore").DependsOn(theScenario.Find("FubuMVC")).ShouldBeFalse();
			theScenario.Find("FubuCore").DependsOn(theScenario.Find("HtmlTags")).ShouldBeFalse();
			theScenario.Find("FubuCore").DependsOn(theScenario.Find("Bottles")).ShouldBeFalse();
			theScenario.Find("FubuMVC").DependsOn(theScenario.Find("FubuMVC.Core.UI")).ShouldBeFalse();
        }
    }
}