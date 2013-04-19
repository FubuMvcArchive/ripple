using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using NuGet;
using ripple.Model;

namespace ripple.Nuget
{
    public class RemoteNuget : IRemoteNuget, DescribesItself
    {
        private readonly INugetDownloader _downloader;
        private readonly Lazy<IEnumerable<Dependency>> _dependencies;

        public RemoteNuget(string name, string version, string url, INugetFeed feed)
        {
            Name = name;
            Version = SemanticVersion.Parse(version);
            _downloader = new UrlNugetDownloader(url);

            _dependencies = new Lazy<IEnumerable<Dependency>>(() => feed.Repository.FindPackage(Name, Version).ImmediateDependencies());
        }

        public RemoteNuget(IPackage package)
        {
            Name = package.Id;
            Version = package.Version;
            _downloader = new RemotePackageDownloader(package);

            _dependencies = new Lazy<IEnumerable<Dependency>>(package.ImmediateDependencies);
        }

        public INugetDownloader Downloader
        {
            get { return _downloader; }
        }

        public string Name { get; private set; }
        public SemanticVersion Version { get; private set; }

		public INugetFile DownloadTo(Solution solution, string directory)
        {
            var file = directory.AppendPath(Filename);
            return _downloader.DownloadTo(solution.Mode, file);
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Version: {1}", Name, Version);
        }

        public string Filename
        {
            get
            {
                if (Version.SpecialVersion.IsEmpty())
                {
                    return "{0}.{1}.nupkg".ToFormat(Name, Version.Version.ToString());
                }

                return "{0}.{1}-{2}.nupkg".ToFormat(Name, Version.Version.ToString(), Version.SpecialVersion);
            }    
        }

        public IEnumerable<Dependency> Dependencies()
        {
            return _dependencies.Value;
        }

        public void Describe(Description description)
	    {
		    description.ShortDescription = "Download {0}".ToFormat(Filename);
	    }
    }
}