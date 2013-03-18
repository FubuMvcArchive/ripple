using System.Collections.Generic;
using System.IO;
using System.Xml;
using FubuCore;
using ripple.New.Nuget;

namespace ripple.New.Model
{
	public class NuGetDependencyStrategy : IDependencyStrategy
	{
		public const string PackagesConfig = "packages.config";

		private readonly IFileSystem _fileSystem = new FileSystem();

		public bool Matches(Project project)
		{
			return _fileSystem.FileExists(project.Directory, PackagesConfig);
		}

		public IEnumerable<Dependency> Read(Project project)
		{
			var document = new XmlDocument();
			document.Load(Path.Combine(project.Directory, PackagesConfig));

			return ReadFrom(document);
		}

		public INugetFile FileFor(string path)
		{
			return new NugetFile(path);
		}

		public void Write(Project project)
		{
			// TODO -- write out the packages.config
			throw new System.NotImplementedException();
		}

		public void RemoveDependencyConfigurations(Project project)
		{
			_fileSystem.DeleteFile(Path.Combine(project.Directory, PackagesConfig));
		}

		public static IEnumerable<Dependency> ReadFrom(XmlDocument document)
		{
			foreach (XmlElement element in document.SelectNodes("//package"))
			{
				yield return ReadFrom(element);
			}
		}

		public static Dependency ReadFrom(XmlElement element)
		{
			return new Dependency(element.GetAttribute("id"), element.GetAttribute("version"), UpdateMode.Fixed);
		}
	}
}