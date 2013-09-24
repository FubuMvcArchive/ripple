using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Model;
using ripple.Model.Conversion;

namespace ripple.Testing.Model
{
	[TestFixture]
	public class NugetDependencyStrategyTester
	{
		[Test]
		public void can_read_and_write_the_packages_config()
		{
			var theFileSystem = new FileSystem();

			theFileSystem.WriteStringToFile(NuGetDependencyStrategy.PackagesConfig, "<?xml version=\"1.0\" encoding=\"utf-8\"?><packages></packages>");
			
			var theSolution = new Solution();
			theSolution.AddDependency(new Dependency("Bottles", "1.0.1.1"));
			theSolution.AddDependency(new Dependency("FubuCore", "1.2.0.1"));

			var theProject = new Project("Test.csproj");
			theProject.AddDependency("Bottles");
			theProject.AddDependency("FubuCore");

			theSolution.AddProject(theProject);

			var theStrategy = new NuGetDependencyStrategy();
			theStrategy.Write(theProject);

			theStrategy
				.Read(theProject)
				.ShouldHaveTheSameElementsAs(
					new Dependency("Bottles", "1.0.1.1"),
					new Dependency("FubuCore", "1.2.0.1")
				);

			theFileSystem.DeleteFile(NuGetDependencyStrategy.PackagesConfig);
		}
	}
}