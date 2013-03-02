using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;

namespace ripple.New.Model
{
	public interface IDependencyReader
	{
		bool Matches(Project project);
		IEnumerable<Dependency> Read(Project project);
	}

	public class RippleDependencyReader : IDependencyReader
	{
		public const string RippleDependenciesConfig = "ripple.dependencies.config";

		private readonly IFileSystem _fileSystem = new FileSystem();

		public bool Matches(Project project)
		{
			return _fileSystem.FileExists(project.Directory, RippleDependenciesConfig);
		}

		public IEnumerable<Dependency> Read(Project project)
		{
			var dependencies = new List<Dependency>();
			_fileSystem.ReadTextFile(Path.Combine(project.Directory, RippleDependenciesConfig), line =>
			{
				if (line.IsEmpty()) return;

				var values = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				var version = string.Empty;

				if (values.Length == 2)
				{
					version = values[1].Trim();
				}

				dependencies.Add(new Dependency(values[0].Trim(), version));
			});
			
			return dependencies;
		}
	}
}