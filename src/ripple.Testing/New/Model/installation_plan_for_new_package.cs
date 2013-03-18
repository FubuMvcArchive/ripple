using FubuTestingSupport;
using NUnit.Framework;
using ripple.New.Model;

namespace ripple.Testing.New.Model
{
	[TestFixture]
	public class installation_plan_for_new_package
	{
		private InstallationPlan thePlan;

		[SetUp]
		public void SetUp()
		{
			thePlan = InstallationScenario.For("FubuMVC.Json", scenario =>
			{
				scenario.AddRemoteDependency("FubuJson", "0.1.1.1");
				scenario.AddRemoteDependency("Newtonsoft.Json", "4.5.9");
			});
		}

		[Test]
		public void verify_the_packages_to_install()
		{
			thePlan.Installations.ShouldHaveTheSameDependenciesAs("FubuMVC.Json", "FubuJson", "Newtonsoft.Json");
		}

		[Test]
		public void no_updates()
		{
			thePlan.Updates.ShouldHaveCount(0);
		}
	}
}