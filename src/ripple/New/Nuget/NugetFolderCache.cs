using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using ripple.New.Model;

namespace ripple.New.Nuget
{
    public class NugetFolderCache : INugetCache
    {
        private readonly string _folder;

        public NugetFolderCache(string folder)
        {
            _folder = folder;
        }

        public void UpdateAll(IEnumerable<IRemoteNuget> nugets)
        {
            var latest = new Cache<string, Version>(name => new Version("0.0.0.0"));
            AllFiles().ToList().GroupBy(x => x.Name).Each(x => {
                var version = x.OrderByDescending(file => file.Version).First().Version.Version;
                latest[x.Key] = version;
            });

            nugets.Each(nuget => {
                var localVersion = latest[nuget.Name];
                var remoteVersion = nuget.Version.Version;

                if (remoteVersion > localVersion)
                {
                    Console.WriteLine("Downloading {0} to {1}", nuget.Filename, _folder);
                    nuget.DownloadTo(_folder);
                }
            });
        }

		public INugetFile Latest(Dependency query)
        {
            IEnumerable<INugetFile> files = AllFiles().Where(x => x.Name == query.Name).ToList();
            if (query.Stability == NugetStability.ReleasedOnly)
            {
                files = files.Where(x => x.Version.SpecialVersion.IsEmpty());
            }

            return files.OrderByDescending(x => x.Version).FirstOrDefault();
        }

        public void Flush()
        {
            new FileSystem().CleanDirectory(_folder);
        }

        public IEnumerable<INugetFile> AllFiles()
        {
            return
                new FileSystem().FindFiles(_folder, new FileSet {Include = "*.nupkg"})
                                .Select(file => new NugetFile(file));
        }

		public INugetFile Find(Dependency query)
        {
            return
                AllFiles()
                    .FirstOrDefault(x => x.Name == query.Name && x.Version.Version.ToString() == query.Version);
        }
    }
}