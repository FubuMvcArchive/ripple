using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using ripple.New.Model;
using ripple.New.Nuget;

namespace ripple.Testing.New.Model
{
	[TestFixture]
	public class SolutionTester
	{
		[Test]
		public void default_feeds()
		{
			new Solution()
				.Feeds
				.ShouldHaveTheSameElementsAs(Feed.Fubu, Feed.NuGetV2, Feed.NuGetV1);
		}

		[Test]
		public void default_storage()
		{
			new Solution().Storage.ShouldBeOfType<RippleStorage>();
		}

		[Test]
		public void default_feed_service()
		{
			new Solution().FeedService.ShouldBeOfType<FeedService>();
		}


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
		public void missing_files()
		{
			var solution = new Solution();
			var storage = MockRepository.GenerateStub<INugetStorage>();

			var q1 = new Dependency("Bottles", "1.0.1.1");
			var q2 = new Dependency("FubuCore", "1.0.1.252");

			storage.Stub(x => x.MissingFiles(solution)).Return(new[] {q1, q2});

			solution.UseStorage(storage);

			solution.MissingNugets().ShouldHaveTheSameElementsAs(q1, q2);
		}

		[Test]
		public void local_dependencies()
		{
			var solution = new Solution();
			var storage = MockRepository.GenerateStub<INugetStorage>();

			var dependencies = new LocalDependencies(new[] {new NugetFile("Bottles.1.0.1.252.nupkg")});
			storage.Stub(x => x.Dependencies(solution)).Return(dependencies);

			solution.UseStorage(storage);

			solution.LocalDependencies().ShouldEqual(dependencies);
		}

		[Test]
		public void valid_for_matching_dependencies()
		{
			var solution = new Solution();
			
			var p1 = new Project("Project1.csproj");
			p1.AddDependency(new Dependency("Bottles", "1.0.0.0"));

			var p2 = new Project("Project2.csproj");
			p2.AddDependency(new Dependency("Bottles", "1.0.0.0"));

			solution.AddProject(p1);
			solution.AddProject(p2);

			solution.AssertIsValid();
		}

		[Test]
		public void valid_for_floated_nugets()
		{
			var solution = new Solution();

			var p1 = new Project("Project1.csproj");
			p1.AddDependency(new Dependency("Bottles"));

			var p2 = new Project("Project2.csproj");
			p2.AddDependency(new Dependency("Bottles"));

			solution.AddProject(p1);
			solution.AddProject(p2);

			solution.AssertIsValid();
		}

		[Test]
		public void invalid_for_mismatched_dependencies()
		{
			var solution = new Solution();

			var p1 = new Project("Project1.csproj");
			p1.AddDependency(new Dependency("Bottles", "1.0.0.0"));

			var p2 = new Project("Project2.csproj");
			p2.AddDependency(new Dependency("Bottles", "0.9.0.0"));

			solution.AddProject(p1);
			solution.AddProject(p2);

			Exception<RippleException>.ShouldBeThrownBy(() => solution.AssertIsValid())
				.HasProblems().ShouldBeTrue();
		}

		[Test]
		public void find_the_dependency_configuration()
		{
			var dependency = new Dependency("Bottles", "1.0.0.0");
			
			var solution = new Solution();
			solution.AddDependency(dependency);

			solution.FindDependency("Bottles").ShouldEqual(dependency);
		}
	}
}