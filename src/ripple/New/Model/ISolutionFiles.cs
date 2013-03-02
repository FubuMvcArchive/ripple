using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FubuCore;

namespace ripple.New.Model
{
	public interface ISolutionFiles
	{
		string RootDir { get; }
		string BuildSupportDir { get; }

		void ForProjects(Action<string> action);

		Solution LoadSolution();
	}

	public class SolutionFiles : ISolutionFiles
	{
		public const string ConfigFile = "ripple.config";

		private readonly IFileSystem _fileSystem;

		public SolutionFiles(IFileSystem fileSystem)
		{
			BuildSupportDir = Assembly.GetExecutingAssembly().Location.ToFullPath();
			RootDir = BuildSupportDir.ParentDirectory().ParentDirectory();

			SrcDir = Path.Combine(RootDir, "src");

			_fileSystem = fileSystem;
		}

		public string RootDir { get; set; }
		public string BuildSupportDir { get; set; }
		public string SrcDir { get; set; }

		public void ForProjects(Action<string> action)
		{
			var csProjSet = new FileSet()
			{
				Include = "*.csproj"
			};

			_fileSystem.FindFiles(SrcDir, csProjSet).Each(action);
		}

		public Solution LoadSolution()
		{
			var file = Path.Combine(RootDir, ConfigFile);
			
			var solution = _fileSystem.LoadFromFile<Solution>(file);
			solution.Path = file;

			return solution;
		}

		public static SolutionFiles Basic()
		{
			return new SolutionFiles(new FileSystem());
		}
	}
}