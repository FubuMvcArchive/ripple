using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FubuCore;
using ripple.New.Classic;

namespace ripple.New.Model
{
	public interface ISolutionFiles
	{
		string RootDir { get; }
		string BuildSupportDir { get; }

		void ForProjects(Action<string> action);
		void ForNuspecs(Action<string> action);

		Solution LoadSolution();

		void FinalizeSolution(Solution solution);
	}

	public class SolutionFiles : ISolutionFiles
	{
		public const string ConfigFile = "ripple.config";

		private readonly IFileSystem _fileSystem;
		private readonly ISolutionLoader _loader;

		public SolutionFiles(IFileSystem fileSystem, ISolutionLoader loader)
		{
			BuildSupportDir = Assembly.GetExecutingAssembly().Location.ToFullPath();
			RootDir = BuildSupportDir.ParentDirectory().ParentDirectory();

			SrcDir = Path.Combine(RootDir, "src");
			PackagingDir = Path.Combine(RootDir, "packaging");

			_fileSystem = fileSystem;
			_loader = loader;
		}

		private void resetDirectories(string root)
		{
			RootDir = root;
			BuildSupportDir = Path.Combine(RootDir, "buildsupport");

			SrcDir = Path.Combine(RootDir, "src");
			PackagingDir = Path.Combine(RootDir, "packaging");
		}

		public string RootDir { get; set; }
		public string BuildSupportDir { get; set; }
		public string PackagingDir { get; set; }
		public string SrcDir { get; set; }

		public void ForProjects(Action<string> action)
		{
			var csProjSet = new FileSet()
			{
				Include = "*.csproj"
			};

			_fileSystem.FindFiles(SrcDir, csProjSet).Each(action);
		}

		public void ForNuspecs(Action<string> action)
		{
			var nuspecSet = new FileSet()
			{
				Include = "*.nuspec"
			};

			_fileSystem.FindFiles(PackagingDir, nuspecSet).Each(action);
		}

		public Solution LoadSolution()
		{
			var file = Path.Combine(RootDir, ConfigFile);

			var solution = _loader.LoadFrom(_fileSystem, file);
			solution.Path = file;

			return solution;
		}

		public void FinalizeSolution(Solution solution)
		{
			_loader.SolutionLoaded(solution);
		}

		public static SolutionFiles Basic()
		{
			return new SolutionFiles(new FileSystem(), new SolutionLoader());
		}

		public static SolutionFiles BasicFromDirectory(string directory)
		{
			var files = Basic();
			files.resetDirectories(directory);

			return files;
		}

		public static SolutionFiles Classic()
		{
			return new SolutionFiles(new FileSystem(), new NuGetSolutionLoader());
		}

		public static SolutionFiles For(SolutionMode mode)
		{
			return mode == SolutionMode.Classic ? Classic() : Basic();
		}
	}
}