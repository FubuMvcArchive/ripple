using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;
using NuGet;
using ripple.Model;

namespace ripple.Nuget
{
    public class FileSystemNugetFeed : INugetFeed
    {
        private readonly string _directory;
        private readonly FubuCore.IFileSystem _fileSystem;
        private readonly NugetStability _stability;

        public FileSystemNugetFeed(string directory, NugetStability stability)
        {
            _directory = directory.ToCanonicalPath();

            Path.GetInvalidPathChars().Each(x =>
            {
                if (_directory.Contains(x))
                {
                    throw new InvalidOperationException("Invalid character in path: {0} ({1})".ToFormat(x, (int)x));
                }
            });

            _stability = stability;
            _fileSystem = new FileSystem();
        }

        public string Directory { get { return _directory; } }

        protected IEnumerable<INugetFile> files
        {
            get
            {
                var nupkgSet = new FileSet
                {
                    Include = "*.nupkg",
                    DeepSearch = false
                };

                return _fileSystem.FindFiles(_directory, nupkgSet).Select(x => new NugetFile(x, SolutionMode.Ripple));
            }
        }

        private IRemoteNuget findMatching(Func<INugetFile, bool> predicate)
        {
            var file = files.FirstOrDefault(predicate);
            if (file == null)
            {
                return null;
            }

            return new FileSystemNuget(file); 
        }

        public IRemoteNuget Find(Dependency query)
        {
            RippleLog.Debug("Searching for {0} in {1}".ToFormat(query, _directory));

            SemanticVersion version;
			if (!SemanticVersion.TryParse(query.Version, out version))
			{
				RippleLog.Debug("Could not find exact for " + query);
				return null;
			}

            return findMatching(nuget => nuget.Name == query.Name && nuget.Version == version);
        }

        public IRemoteNuget FindLatest(Dependency query)
        {
            RippleLog.Debug("Searching for latest of {0} in {1}".ToFormat(query, _directory));

            var nugets = files
                .Where(x => x.Name == query.Name && (!x.IsPreRelease || (x.IsPreRelease && query.DetermineStability(_stability) == NugetStability.Anything)))
                .ToList();
                
            var nuget = nugets
                .OrderByDescending(x => x.Version)
                .FirstOrDefault();

            if (nuget == null)
            {
                return null;
            }

            return new FileSystemNuget(nuget);
        }

        public IPackageRepository Repository { get; private set; }

        public override string ToString()
        {
            return _directory;
        }
    }

    public class FloatingFileSystemNugetFeed : FileSystemNugetFeed, IFloatingFeed
    {
        public FloatingFileSystemNugetFeed(string directory, NugetStability stability) 
            : base(directory, stability)
        {
        }

        public IEnumerable<IRemoteNuget> GetLatest()
        {
            var nugets = new List<INugetFile>();

            RippleLog.Debug("Retrieving all latest from " + Directory);
            
            files
                .GroupBy(x => x.Name)
                .Each(x =>
                {
                    var latest = x.OrderByDescending(n => n.Version).First();
                    nugets.Add(latest);
                });

            return nugets
                .Select(x => new FileSystemNuget(x))
                .OrderBy(x => x.Name);
        }
    }
}