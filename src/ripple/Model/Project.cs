using System;
using System.IO;
using FubuCore;
using FubuCore.Descriptions;
using ripple.MSBuild;

namespace ripple.Model
{
	public class Project : DescribesItself
	{
		private readonly DependencyCollection _dependencies = new DependencyCollection();
		private readonly Lazy<CsProjFile> _csProj;

		public Project(string filePath)
		{
			FilePath = filePath;
			Name = Path.GetFileNameWithoutExtension(filePath);
			Directory = filePath.DirectoryPath();

			_csProj = new Lazy<CsProjFile>(() => new CsProjFile(filePath));
		}

		public string Name { get; private set; }
		public string Directory { get; private set; }
		public string FilePath { get; private set; }
		public CsProjFile CsProj { get { return _csProj.Value; } }
		public Solution Solution { get; set; }

		public DependencyCollection Dependencies
		{
			get { return _dependencies; }
		}

		public void AddDependency(Dependency dependency)
		{
			_dependencies.Add(dependency);
		}

		protected bool Equals(Project other)
		{
			return string.Equals(FilePath, other.FilePath);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Project)obj);
		}

		public override int GetHashCode()
		{
			return FilePath.GetHashCode();
		}

		public void Clean(IFileSystem system)
		{
			system.CleanWithTracing(Directory.AppendPath("bin"));
			system.CleanWithTracing(Directory.AppendPath("obj"));
		}

		public void Describe(Description description)
		{
			description.Title = "Project \"{0}\"".ToFormat(Name);
			description.ShortDescription = FilePath;

			var list = description.AddList("Dependencies", Dependencies);
			list.Label = "Dependencies";
		}
	}
}