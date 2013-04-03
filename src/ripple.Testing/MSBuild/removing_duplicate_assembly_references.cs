using System.Linq;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.MSBuild;

namespace ripple.Testing.MSBuild
{
	[TestFixture]
	public class removing_duplicate_assembly_references
	{
		private CsProjFile theCsProj;
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

			theCsProj.RemoveDuplicateReferences();
			theCsProj.Write();

			theCsProj = null;
			theCsProj = new CsProjFile(theFilename);
		}

		[Test]
		public void removes_by_matching_on_just_the_assembly_name()
		{
			theCsProj.FindReferenceNodes().Count().ShouldNotEqual(0);

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

	[TestFixture]
	public class removing_duplicate_assembly_references_2
	{
		private CsProjFile theCsProj;
		private string theFilename;

		[SetUp]
		public void SetUp()
		{
			theFilename = "Test.txt";
			var stream = GetType()
				.Assembly
				.GetManifestResourceStream(GetType(), "ProjectWithDuplicateRefs.txt");

			new FileSystem().WriteStreamToFile(theFilename, stream);

			theCsProj = new CsProjFile(theFilename);

			theCsProj
				.References
				.ShouldHaveTheSameElementKeysAs(new[]
				{
					"Bottles", 
					"FubuCore",
					"FubuLocalization",
					"FubuMVC.Core",
					"FubuMVC.Core.Assets",
					"FubuMVC.Core.UI",
					"FubuMVC.Core.View",
					"FubuMVC.JQueryUI",
					"FubuMVC.Media",
					"FubuMVC.StructureMap",
					"FubuTestingSupport",
					"HtmlTags",
					"Microsoft.CSharp",
					"nunit.framework",
					"nunit.framework, Version=2.5.10.11092, Culture=neutral, PublicKeyToken=96d09a1eb7f44a77, processorArchitecture=MSIL",
					"nunit.mocks",
					"nunit.mocks, Version=2.5.10.11092, Culture=neutral, PublicKeyToken=96d09a1eb7f44a77, processorArchitecture=MSIL",
					"pnunit.framework",
					"pnunit.framework, Version=1.0.4109.34242, Culture=neutral, processorArchitecture=MSIL",
					"Rhino.Mocks",
					"Rhino.Mocks, Version=3.6.0.0, Culture=neutral, PublicKeyToken=0b3305902db7183f, processorArchitecture=MSIL",
					"StructureMap",
					"StructureMap.AutoMocking",
					"StructureMap.AutoMocking, Version=2.6.3.0, Culture=neutral, PublicKeyToken=e60ad81abae3c223, processorArchitecture=MSIL",
					"System",
					"System.Core",
					"System.Data",
					"System.Data.DataSetExtensions",
					"System.Xml",
					"System.Xml.Linq"
				}, x => x.Name);

			theCsProj.RemoveDuplicateReferences();
			theCsProj.Write();

			theCsProj = null;
			theCsProj = new CsProjFile(theFilename);
		}

		[Test]
		public void removes_by_matching_on_just_the_assembly_name()
		{
			theCsProj.FindReferenceNodes().Count().ShouldNotEqual(0);

			theCsProj
				.References
				.ShouldHaveTheSameElementKeysAs(new[]
				{
					"Bottles", 
					"FubuCore",
					"FubuLocalization",
					"FubuMVC.Core",
					"FubuMVC.Core.Assets",
					"FubuMVC.Core.UI",
					"FubuMVC.Core.View",
					"FubuMVC.JQueryUI",
					"FubuMVC.Media",
					"FubuMVC.StructureMap",
					"FubuTestingSupport",
					"HtmlTags",
					"Microsoft.CSharp",
					"nunit.framework",
					"nunit.mocks",
					"pnunit.framework",
					"Rhino.Mocks",
					"StructureMap",
					"StructureMap.AutoMocking",
					"System",
					"System.Core",
					"System.Data",
					"System.Data.DataSetExtensions",
					"System.Xml",
					"System.Xml.Linq"
				}, x => x.Name);
		}
	}
}