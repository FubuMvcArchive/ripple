using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FubuCore;
using ripple.Classic;

namespace ripple.Model
{
	public interface ISolutionFiles
	{
		string RootDir { get; }
		string BuildSupportDir { get; }

		void ForProjects(Solution solution, Action<string> action);
		void ForNuspecs(Solution solution, Action<string> action);

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

			_fileSystem = fileSystem;
			_loader = loader;
		}

		private void resetDirectories(string root)
		{
			RootDir = root;
			BuildSupportDir = Path.Combine(RootDir, "buildsupport");
		}

		public string RootDir { get; set; }
		public string BuildSupportDir { get; set; }

		public ISolutionLoader Loader { get { return _loader; } }

		public SolutionMode Mode
		{
			get
			{
				if(_loader is SolutionLoader) return SolutionMode.Ripple;
				return SolutionMode.Classic;
			}
		}

		public void ForProjects(Solution solution, Action<string> action)
		{
			var csProjSet = new FileSet()
			{
				Include = "*.csproj"
			};

			var targetDir = Path.Combine(solution.Directory, solution.SourceFolder);
			_fileSystem.FindFiles(targetDir, csProjSet).Each(action);
		}

		public void ForNuspecs(Solution solution, Action<string> action)
		{
			var nuspecSet = new FileSet()
			{
				Include = "*.nuspec"
			};

			var targetDir = Path.Combine(solution.Directory, solution.NugetSpecFolder);
			_fileSystem.FindFiles(targetDir, nuspecSet).Each(action);
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

		public static SolutionFiles FromDirectory(string directory)
		{
			var nugetConfigs = new FileSet
			{
				Include = NuGetDependencyStrategy.PackagesConfig,
				Exclude = "fubu-content/*.*",
				DeepSearch = true
			};

			var isClassicMode = false;
			var configFiles = new FileSystem().FindFiles(directory, nugetConfigs);
			foreach (var file in configFiles)
			{
				var configFile = file.ToFullPath();
				// TODO -- Maybe configure this somehow
				if (!configFile.Contains("fubu-content") && !configFile.Contains("gems"))
				{
					isClassicMode = true;
					RippleLog.Info("Classic Mode Detected: ({0})".ToFormat(configFiles.Select(x => x.ToFullPath()).Join(";")));
					break;
				}
			}

			var files =  isClassicMode ? Classic() : Basic();
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
