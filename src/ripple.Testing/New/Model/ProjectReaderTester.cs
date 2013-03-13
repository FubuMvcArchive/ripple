using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using ripple.New.Model;

namespace ripple.Testing.New.Model
{
	[TestFixture]
	public class ProjectReaderTester
	{
		private IDependencyStrategy r1;
		private IDependencyStrategy r2;
		private ProjectReader theReader;

		private Dependency d1;
		private Dependency d2;

		private Project theProject;
		
		[SetUp]
		public void SetUp()
		{
			r1 = MockRepository.GenerateStub<IDependencyStrategy>();
			r2 = MockRepository.GenerateStub<IDependencyStrategy>();

			d1 = new Dependency("FubuCore", "1.0.0.215");
			d2 = new Dependency("Bottles", "1.0.0.212");

			var project = new Project("MyProject.csproj");

			r1.Stub(x => x.Matches(project)).Return(false);
			r2.Stub(x => x.Matches(project)).Return(true);

			r2.Stub(x => x.Read(project)).Return(new[] {d1, d2});

			theReader = new ProjectReader(new[] {r1, r2});
			theProject = theReader.Read("MyProject.csproj");
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