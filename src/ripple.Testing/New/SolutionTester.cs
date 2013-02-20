using FubuTestingSupport;
using NUnit.Framework;
using ripple.New;

namespace ripple.Testing.New
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
	}
}