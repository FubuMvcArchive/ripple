using System;
using System.Linq;
using NuGet;
using ripple.New.Model;

namespace ripple.New.Nuget
{
    public class NugetFeed : INugetFeed
    {
        private readonly IPackageRepository _repository;
        private readonly string _url;

        public NugetFeed(string url)
        {
            _url = url.TrimEnd('/');
            _repository = new PackageRepositoryFactory().CreateRepository(_url);
        }

        public string Url
        {
            get { return _url; }
        }

		public IRemoteNuget Find(Dependency query)
        {
            var versionSpec = new VersionSpec(SemanticVersion.Parse(query.Version));
            var package = _repository.FindPackages(query.Name, versionSpec, query.Stability == NugetStability.Anything, true).SingleOrDefault();

            if (package == null)
            {
	            return null;
            }
            
            return new RemoteNuget(package);
            
            
        }


		public IRemoteNuget FindLatest(Dependency query)
        {
			RippleLog.Debug("Searching for " + query);
            var candidates = _repository.Search(query.Name, query.Stability == NugetStability.Anything)
                                        .Where(x => x.Id == query.Name).OrderBy(x => x.Id).ToList();


            var candidate = candidates.FirstOrDefault(x => x.IsAbsoluteLatestVersion)
                            ?? candidates.FirstOrDefault(x => x.IsLatestVersion);

            if (candidate == null)
            {
	            return null;
            }

            return new RemoteNuget(candidate);
        }
    }
}