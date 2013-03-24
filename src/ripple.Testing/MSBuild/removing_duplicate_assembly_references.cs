using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.Directives;
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
			theCsProj.FindReferenceNodes().Count.ShouldNotEqual(0);

			// Opting of Mono for now. Sigh.
			if (DirectiveRunner.IsUnix())
				return;			

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