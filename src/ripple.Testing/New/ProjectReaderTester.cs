using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using ripple.New;
using ripple.New.Model;

namespace ripple.Testing.New
{
	[TestFixture]
	public class ProjectReaderTester
	{
		private IDependencyReader r1;
		private IDependencyReader r2;
		private ProjectReader theReader;

		private NugetDependency d1;
		private NugetDependency d2;

		private Project theProject;
		
		[SetUp]
		public void SetUp()
		{
			r1 = MockRepository.GenerateStub<IDependencyReader>();
			r2 = MockRepository.GenerateStub<IDependencyReader>();

			d1 = new NugetDependency("FubuCore", "1.0.0.215");
			d2 = new NugetDependency("Bottles", "1.0.0.212");

			var project = new Project("MyProject.csproj");

			r1.Stub(x => x.Matches(project, "MyProject")).Return(false);
			r2.Stub(x => x.Matches(project, "MyProject")).Return(true);

			r2.Stub(x => x.Read(project, "MyProject")).Return(new[] {d1, d2});

			theReader = new ProjectReader(new[] {r1, r2});
			theProject = theReader.Read(new ProjectFiles("MyProject.csproj", "MyProject"));
		}

		[Test]
		public void builds_the_project()
		{
			theProject.Name.ShouldEqual("MyProject");
			theProject.FilePath.ShouldEqual("MyProject.csproj");
		}

		[Test]
		public void reads_the_dependencies()
		{
			theProject.Dependencies.ShouldHaveTheSameElementsAs(d1, d2);
		}
	}
}