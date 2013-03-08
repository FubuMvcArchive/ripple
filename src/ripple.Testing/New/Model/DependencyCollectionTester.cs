using FubuTestingSupport;
using NUnit.Framework;
using ripple.New.Model;

namespace ripple.Testing.New.Model
{
	[TestFixture]
	public class DependencyCollectionTester
	{
		private DependencyCollection theSolutionDependencies;

		[SetUp]
		public void SetUp()
		{
			theSolutionDependencies = new DependencyCollection();
		}

		[Test]
		public void finds_the_dependency_from_a_child_collection()
		{
			var projectDependency = new Dependency("Bottles");
			var projectDependencies = new DependencyCollection();
			projectDependencies.Add(projectDependency);

			theSolutionDependencies.AddChild(projectDependencies);

			theSolutionDependencies.Find("Bottles").ShouldEqual(projectDependency);
		}

		[Test]
		public void uses_the_top_most_dependency()
		{
			var projectDependency = new Dependency("StructureMap");
			var projectDependencies = new DependencyCollection();
			projectDependencies.Add(projectDependency);

			var solutionDependency = new Dependency("StructureMap", "2.6.3", UpdateMode.Fixed);
			theSolutionDependencies.Add(solutionDependency);
			theSolutionDependencies.AddChild(projectDependencies);

			theSolutionDependencies.Find("StructureMap").ShouldEqual(solutionDependency);
		}

		[Test]
		public void enumerating_aggregates_the_dependencies()
		{
			var child1 = new DependencyCollection();
			child1.Add(new Dependency("Bottles"));
			child1.Add(new Dependency("FubuCore"));

			var child2 = new DependencyCollection();
			child2.Add(new Dependency("Bottles"));
			child2.Add(new Dependency("FubuCore"));
			child2.Add(new Dependency("FubuTestingSupport", "0.9.2.4"));
			child2.Add(new Dependency("RhinoMocks", "3.6.1"));

			theSolutionDependencies.Add(new Dependency("Bottles", "1.0.1.2"));
			theSolutionDependencies.Add(new Dependency("FubuCore", "1.0.1.5"));
			
			theSolutionDependencies.AddChild(child1);
			theSolutionDependencies.AddChild(child2);

			var expected = new[]
			{
				new Dependency("Bottles", "1.0.1.2"),
				new Dependency("FubuCore", "1.0.1.5"),
				new Dependency("FubuTestingSupport", "0.9.2.4"),
				new Dependency("RhinoMocks", "3.6.1")
			};

			theSolutionDependencies.ShouldHaveTheSameElementsAs(expected);
		}
	}
}