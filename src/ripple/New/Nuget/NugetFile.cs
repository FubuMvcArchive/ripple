using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FubuCore;
using FubuCore.Descriptions;
using NuGet;

namespace ripple.New.Nuget
{
	public class NugetName
	{
		public string Name { get; private set; }
		public SemanticVersion Version { get; private set; }

		public static NugetName Parse(string input)
		{
			var index = 0;
			var periodFound = false;

			for (var i = 0; i < input.Length; i++)
			{
				var character = input[i];
				if (character == '.')
				{
					periodFound = true;
					continue;
				}

				if (periodFound && char.IsNumber(character))
				{
					index = i;
					break;
				}
			}

			if (index == 0)
			{
				throw new InvalidOperationException("Invalid package name");
			}

			return new NugetName
			{
				Name = input.Substring(0, index - 1),
				Version = SemanticVersion.Parse(input.Substring(index))
			};
		}
	}

    public class NugetFile : INugetFile, DescribesItself
    {
        private readonly string _path;

        public NugetFile(string path)
        {
            _path = path;

            var file = Path.GetFileNameWithoutExtension(path);
	        var result = NugetName.Parse(file);

	        Name = result.Name;
	        Version = result.Version;
	        IsPreRelease = Version.SpecialVersion.IsNotEmpty();
        }

		public string FileName { get { return _path; } }
	    public string Name { get; private set; }
        public SemanticVersion Version { get; private set; }
        public bool IsPreRelease { get; private set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, Version: {1}, IsPreRelease: {2}", Name, Version, IsPreRelease);
        }

		public virtual string ExplodedDirectory(string directory)
		{
			return directory.AppendPath(Name, Version.ToString());
		}

        public IPackage ExplodeTo(string directory)
        {
	        var explodedDirectory = ExplodedDirectory(directory);
            var fileSystem = new FileSystem();
            fileSystem.CreateDirectory(explodedDirectory);
            fileSystem.CleanDirectory(explodedDirectory);

            var package = new ZipPackage(_path);

            package.GetFiles().Each(file => {
                var target = explodedDirectory.AppendPath(file.Path);
                fileSystem.CreateDirectory(target.ParentDirectory());
                
                using (var stream = new FileStream(target, FileMode.Create, FileAccess.Write))
                {
                    file.GetStream().CopyTo(stream);
                }
            });

            fileSystem.CopyToDirectory(_path, explodedDirectory);

			fileSystem.DeleteFile(_path);

            var repository = new LocalPackageRepository(directory);
            return repository.FindPackagesById(Name).Single();
        }

	    public INugetFile CopyTo(string directory)
	    {
		    var target = Path.Combine(directory, Path.GetFileName(_path));
			new FileSystem().Copy(_path, target);

			return new NugetFile(target);
	    }

		protected virtual INugetFile createCopy(string file)
		{
			return new NugetFile(file);
		}

	    public void Describe(Description description)
	    {
		    description.Title = Name;
		    description.ShortDescription = "Version: {0}, IsPreRelease: {1}".ToFormat(Version, IsPreRelease);
	    }
    }

	public class RippleNugetFile : NugetFile
	{
		public RippleNugetFile(string path) : base(path)
		{
		}

		public override string ExplodedDirectory(string directory)
		{
			return directory.AppendPath(Name);
		}

		protected override INugetFile createCopy(string file)
		{
			return new RippleNugetFile(file);
		}
	}
}