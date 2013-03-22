using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
{
	[TestFixture]
	public class installation_plan_for_package_with_existing_dependencies_with_floating_updates_same_version
	{
		private InstallationPlan thePlan;

		[SetUp]
		public void SetUp()
		{
			thePlan = InstallationScenario.For("FubuMVC.Json", scenario =>
				{
					scenario.AddRemoteDependency("Newtonsoft.Json", "4.5.9");
					scenario.AddLocalDependency("Newtonsoft.Json", "4.5.9", UpdateMode.Float);
				});
		}

		[Test]
		public void no_dependencies_to_install()
		{
			thePlan.Installations.ShouldHaveTheSameDependenciesAs("FubuMVC.Json");
		}

		[Test]
		public void no_packages_to_update()
		{
			thePlan.Updates.ShouldHaveCount(0);
		}
	}
}