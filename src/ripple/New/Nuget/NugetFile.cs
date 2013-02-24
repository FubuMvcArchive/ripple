using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using NuGet;

namespace ripple.New.Nuget
{
	// TODO -- Maybe we have a separate implementation to do the NuGet style?
    public class NugetFile : INugetFile
    {
        private readonly string _path;

        public NugetFile(string path)
        {
            _path = path;

            var file = Path.GetFileNameWithoutExtension(path);
            var parts = file.Split('.');
            Name = parts.Reverse().Skip(4).Reverse().Join(".");

            var versionParts = parts.Reverse().Take(4).Reverse().ToList();

            Version = SemanticVersion.Parse(versionParts.Join("."));

            IsPreRelease = Version.SpecialVersion.IsNotEmpty();
        }

        public string Name { get; private set; }
        public SemanticVersion Version { get; private set; }
        public bool IsPreRelease { get; private set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, Version: {1}, IsPreRelease: {2}", Name, Version, IsPreRelease);
        }

        public IPackage ExplodeTo(string directory)
        {
            var explodedDirectory = directory.AppendPath(Name);
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
    }
}