using FubuTestingSupport;
using NUnit.Framework;
using ripple.New.Model;

namespace ripple.Testing.New.Model
{
	[TestFixture]
	public class SolutionTester
	{
		[Test]
		public void adding_a_project_sets_the_solution()
		{
			var solution = new Solution();
			var project = new Project("MyProject.csproj");

			solution.AddProject(project);
			solution.Projects.ShouldHaveTheSameElementsAs(project);

			project.Solution.ShouldBeTheSameAs(solution);
		}

		[Test]
		public void all_dependencies()
		{
			var repository = new Solution();
			var d1 = new Dependency("D1");
			var d2 = new Dependency("D2");

			var p1 = new Project("Project1.csproj");
			p1.AddDependency(d1);

			var p2 = new Project("Project2.csproj");
			p2.AddDependency(d2);

			repository.AddProject(p1);
			repository.AddProject(p2);

			repository.AllDependencies().ShouldHaveTheSameElementsAs(d1, d2);
		}
	}
}