using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FubuCore;

namespace ripple.New
{
	public interface ISolutionFiles
	{
		string RootDir { get; }
		string BuildSupportDir { get; }

		void ForProjects(Action<ProjectFiles> action);

		Solution LoadSolution();
	}

	public class SolutionFiles : ISolutionFiles
	{
		public const string ConfigFile = "ripple.config";

		private readonly IFileSystem _fileSystem;

		public SolutionFiles()
		{
			BuildSupportDir = Assembly.GetExecutingAssembly().Location.ToFullPath();
			RootDir = BuildSupportDir.ParentDirectory().ParentDirectory();

			SrcDir = Path.Combine(RootDir, "src");

			_fileSystem = new FileSystem();
		}

		public string RootDir { get; private set; }
		public string BuildSupportDir { get; private set; }
		public string SrcDir { get; private set; }

		public void ForProjects(Action<ProjectFiles> action)
		{
			var csProjSet = new FileSet()
			{
				Include = "*.csproj"
			};

			_fileSystem.FindFiles(SrcDir, csProjSet).Each(file =>
			{
				var projectFiles = new ProjectFiles(file, file.DirectoryPath());
				action(projectFiles);
			});
		}

		public Solution LoadSolution()
		{
			var file = Path.Combine(RootDir, ConfigFile);
			
			var solution = _fileSystem.LoadFromFile<Solution>(file);
			solution.Path = file;

			return solution;
		}
	}
}