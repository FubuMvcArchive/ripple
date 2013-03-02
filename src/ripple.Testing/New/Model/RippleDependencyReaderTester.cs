using System.Collections.Generic;
using System.Text;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.New.Model;

namespace ripple.Testing.New.Model
{
	[TestFixture]
	public class RippleDependencyReaderTester
	{
		private RippleDependencyReader theDependencyReader;
		private Project theProject;

		[SetUp]
		public void SetUp()
		{
			theDependencyReader = new RippleDependencyReader();
			theProject = new Project("TestProject.csproj");

			var content = new StringBuilder()
				.AppendLine("Bottles")
				.AppendLine("FubuCore,1.0.1.250")
				.AppendLine("FubuMVC.Core,1.0.1.252")
				.ToString();

			new FileSystem().WriteStringToFile(RippleDependencyReader.RippleDependenciesConfig, content);
		}

		[Test]
		public void reads_the_dependencies()
		{
			var dependencies = new List<Dependency>
			{
				new Dependency("Bottles"),
				new Dependency("FubuCore", "1.0.1.250"),
				new Dependency("FubuMVC.Core","1.0.1.252")
			};

			theDependencyReader.Read(theProject).ShouldHaveTheSameElementsAs(dependencies);
		}
	}
}