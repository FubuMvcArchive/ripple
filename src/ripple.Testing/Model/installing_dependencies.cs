using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
{
	[TestFixture]
	public class installing_solution_level_dependencies
	{
		private Solution _solution;

		private Solution theSolution
		{
			get
			{
				if (_solution == null)
				{
					_solution = DependencyInstallationScenario.Create(scenario =>
					{
						scenario.SolutionDependency("Bottles");
						scenario.SolutionDependency("FubuCore");

						scenario.ProjectDependency("Project1", "ProjectDependency1");
						scenario.ProjectDependency("Project2", "ProjectDependency2");
					});
				}

				return _solution;
			}
		}

		[Test]
		public void only_installs_to_the_solution()
		{
			var installation = PackageInstallation.ForSolution(new Dependency("HtmlTags"));
			installation.InstallTo(theSolution);

			theSolution.Nugets.ShouldHaveTheSameDependenciesAs("Bottles", "FubuCore", "HtmlTags");
			theSolution.FindProject("Project1").Dependencies.ShouldHaveTheSameDependenciesAs("ProjectDependency1");
			theSolution.FindProject("Project2").Dependencies.ShouldHaveTheSameDependenciesAs("ProjectDependency2");
		}
	}

	[TestFixture]
	public class installing_a_floated_dependency_to_an_individual_project
	{
		private Solution _solution;

		private Solution theSolution
		{
			get
			{
				if (_solution == null)
				{
					_solution = DependencyInstallationScenario.Create(scenario =>
					{
						scenario.SolutionDependency("Bottles");
						scenario.SolutionDependency("FubuCore");

						scenario.ProjectDependency("Project1", "ProjectDependency1");
						scenario.ProjectDependency("Project2", "ProjectDependency2");
					});
				}

				return _solution;
			}
		}

		[Test]
		public void only_installs_to_the_specific_project()
		{
			var installation = PackageInstallation.ForProject("Project1", new Dependency("HtmlTags"));
			installation.InstallTo(theSolution);

			theSolution.Nugets.ShouldHaveTheSameDependenciesAs("Bottles", "FubuCore");

			theSolution.FindProject("Project1").Dependencies.ShouldHaveTheSameDependenciesAs("HtmlTags", "ProjectDependency1");
			theSolution.FindProject("Project2").Dependencies.ShouldHaveTheSameDependenciesAs("ProjectDependency2");
		}
	}

	[TestFixture]
	public class installing_a_fixed_version_of_a_dependency_to_an_individual_project
	{
		private Solution _solution;

		private Solution theSolution
		{
			get
			{
				if (_solution == null)
				{
					_solution = DependencyInstallationScenario.Create(scenario =>
					{
						scenario.ProjectDependency("Project1", "HtmlTags");
						scenario.ProjectDependency("Project2", "HtmlTags");
					});

					var installation = PackageInstallation.ForProject("Project1", new Dependency("FubuCore", "1.2.1.1", UpdateMode.Fixed));
					installation.InstallTo(_solution);
				}

				return _solution;
			}
		}

		[Test]
		public void installs_the_dependency_name_to_the_project()
		{
			var fubucore = theSolution.FindProject("Project1").Dependencies.Find("FubuCore");
			fubucore.IsFloat().ShouldBeTrue();
		}

		[Test]
		public void keeps_the_version_information_at_the_solution_level()
		{
			theSolution.FindDependency("FubuCore").Version.ShouldEqual("1.2.1.1");
		}
	}
}