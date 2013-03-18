using FubuTestingSupport;
using NUnit.Framework;
using ripple.New.Model;

namespace ripple.Testing.New.Model
{
	[TestFixture]
	public class installation_plan_for_package_with_existing_dependencies_with_floating_updates
	{
		private InstallationPlan thePlan;

		[SetUp]
		public void SetUp()
		{
			thePlan = InstallationScenario.For("FubuMVC.Json", scenario =>
			{
				scenario.AddRemoteDependency("Newtonsoft.Json", "4.5.9");

				scenario.AddLocalDependency("Newtonsoft.Json", "4.5.1", UpdateMode.Float);
			});
		}

		[Test]
		public void no_dependencies_to_install()
		{
			thePlan.Installations.ShouldHaveTheSameDependenciesAs("FubuMVC.Json");
		}

		[Test]
		public void updates_the_local_dependency()
		{
			thePlan.Updates.ShouldHaveTheSameDependenciesAs("Newtonsoft.Json");
		}
	}

	[TestFixture]
	public class installation_plan_for_package_with_existing_fixed_dependencies_with_updates
	{
		private InstallationPlan thePlan;

		[SetUp]
		public void SetUp()
		{
			thePlan = InstallationScenario.For("FubuMVC.Json", scenario =>
			{
				scenario.AddRemoteDependency("Newtonsoft.Json", "4.5.9");
				scenario.AddLocalDependency("Newtonsoft.Json", "4.5.1", UpdateMode.Fixed);
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