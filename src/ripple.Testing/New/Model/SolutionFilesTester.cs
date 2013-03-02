using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using ripple.New.Model;

namespace ripple.Testing.New.Model
{
	[TestFixture]
	public class SolutionFilesTester
	{
		private SolutionFiles theSolutionFiles;
		private IFileSystem theFileSystem;

		[SetUp]
		public void SetUp()
		{
			theFileSystem = new FileSystem();
			theFileSystem.CreateDirectory("SolutionFiles");

			theSolutionFiles = new SolutionFiles(theFileSystem);
			theSolutionFiles.RootDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SolutionFiles");
			
			theFileSystem.CreateDirectory("SolutionFiles", "src");
			theSolutionFiles.SrcDir = Path.Combine(theSolutionFiles.RootDir, "src");

			theFileSystem.CreateDirectory("SolutionFiles", "src", "Project1");
			theFileSystem.CreateDirectory("SolutionFiles", "src", "Project2");

			theFileSystem.WriteStringToFile(Path.Combine("SolutionFiles", "src", "Project1", "Project1.csproj"), "test");
			theFileSystem.WriteStringToFile(Path.Combine("SolutionFiles", "src", "Project2", "Project2.csproj"), "test");
		}

		[TearDown]
		public void TearDown()
		{
			theFileSystem.DeleteDirectory("SolutionFiles");
		}

		[Test]
		public void reads_the_projects()
		{
			var projects = new List<string>();
			theSolutionFiles.ForProjects(projects.Add);

			var project1 = Path.Combine(theSolutionFiles.SrcDir, "Project1", "Project1.csproj");
			var project2 = Path.Combine(theSolutionFiles.SrcDir, "Project2", "Project2.csproj");

			projects.ShouldHaveTheSameElementsAs(project1, project2);
		}
	}
}