using System.Linq;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.MSBuild;
using ripple.Model;

namespace ripple.Testing.MSBuild
{
	[TestFixture]
	public class removing_duplicate_assembly_references
	{
		private CsProjFile theCsProj;
		private Project theProject;
		private string theFilename;

		[SetUp]
		public void SetUp()
		{
			theFilename = "Bottles.txt";
			var stream = GetType()
				.Assembly
				.GetManifestResourceStream(GetType(), "ProjectTemplate.txt");

			new FileSystem().WriteStreamToFile(theFilename, stream);

			theCsProj = new CsProjFile(theFilename);

			theCsProj
				.References
				.ShouldHaveTheSameElementKeysAs(new[]
				{
					"FubuCore", 
					"Ionic.Zip", 
					"Ionic.Zip, Version=1.9.1.8, Culture=neutral, processorArchitecture=MSIL",
					"System",
					"System.Core",
					"System.Web",
					"System.Xml"
				}, x => x.Name);

			theProject = new Project("Test");
			theProject.AddDependency("FubuCore");
			theProject.AddDependency("Ionic.Zip");

			theCsProj.RemoveDuplicateReferences(theProject);
		}

		[Test]
		public void removes_by_matching_on_just_the_assembly_name()
		{
			theCsProj
				.References
				.ShouldHaveTheSameElementKeysAs(new[]
				{
					"FubuCore", 
					"Ionic.Zip",
					"System",
					"System.Core",
					"System.Web",
					"System.Xml"
				}, x => x.Name);
		}
	}
}