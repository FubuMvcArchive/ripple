using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;

namespace ripple.Testing.Model
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

	[TestFixture]
	public class tracking_DependencyCollection_changes
	{
		private DependencyCollection theCollection;

		[SetUp]
		public void SetUp()
		{
			theCollection = new DependencyCollection();

			theCollection.Add(new Dependency("Bottles", "1.0.0.1"));
			theCollection.Add(new Dependency("structuremap", "2.6.3", UpdateMode.Fixed));

			theCollection.MarkRead();
		}

		[Test]
		public void no_changes()
		{
			theCollection.HasChanges().ShouldBeFalse();
		}

		[Test]
		public void changes_when_version_changes()
		{
			theCollection.Find("Bottles").Version = "1.1.0.1";
			theCollection.HasChanges().ShouldBeTrue();
		}

		[Test]
		public void changes_when_update_mode_changes()
		{
			theCollection.Find("structuremap").Float();
			theCollection.HasChanges().ShouldBeTrue();
		}

		[Test]
		public void changes_for_addition()
		{
			theCollection.Add(new Dependency("FubuCore"));
			theCollection.HasChanges().ShouldBeTrue();
		}
	}
}