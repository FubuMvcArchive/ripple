using FubuTestingSupport;
using NUnit.Framework;
using ripple.New.Model;

namespace ripple.Testing.New.Model
{
	[TestFixture]
	public class updating_solution_dependencies
	{
		private Solution theSolution;

		[SetUp]
		public void SetUp()
		{
			theSolution = new Solution();
			theSolution.AddDependency(new Dependency("Bottles", "1.0.0.0"));
			theSolution.AddDependency(new Dependency("FubuCore", "1.0.0.0"));

			theSolution.Update("Bottles", "1.1.0.0");
			theSolution.Update("FubuCore", "1.2.0.1");
		}

		[Test]
		public void updates_the_versions()
		{
			theSolution.Dependencies.Find("Bottles").Version.ShouldEqual("1.1.0.0");
			theSolution.Dependencies.Find("FubuCore").Version.ShouldEqual("1.2.0.1");
		}
	}

	[TestFixture]
	public class updating_project_dependencies
	{
		private Solution theSolution;
		private Project theProject;

		[SetUp]
		public void SetUp()
		{
			theSolution = new Solution();

			theProject = new Project("TestProject.csproj");
			theProject.AddDependency(new Dependency("Bottles", "1.0.0.0"));
			theProject.AddDependency(new Dependency("FubuCore", "1.0.0.0"));

			theSolution.AddProject(theProject);

			theSolution.Update("Bottles", "1.1.0.0");
			theSolution.Update("FubuCore", "1.2.0.1");
		}

		[Test]
		public void updates_the_versions()
		{
			theSolution.Dependencies.Find("Bottles").Version.ShouldEqual("1.1.0.0");
			theSolution.Dependencies.Find("FubuCore").Version.ShouldEqual("1.2.0.1");
		}
	}

	public static class SolutionExtensions
	{
		public static void Update(this Solution solution, string name, string version)
		{
			solution.Update(new StubNugetFile(new Dependency(name, version)));
		}
	}
}