using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using NuGet;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Testing.Model
{
	public class StubFeedProvider : IFeedProvider
	{
		private readonly Cache<Feed, StubFeed> _feeds;

		public StubFeedProvider()
		{
			_feeds = new Cache<Feed, StubFeed>(feed => new StubFeed());
		}

		public INugetFeed For(Feed feed)
		{
			return _feeds[feed];
		}
	}

	public class StubFeed : INugetFeed
	{
		private readonly IList<IRemoteNuget> _nugets = new List<IRemoteNuget>();

		public StubFeed()
		{
			UseRepository(new StubPackageRepository());
		}

		public StubFeed Add(string name, string version)
		{
			return Add(new Dependency(name, version));
		}

		public StubFeed Add(Dependency dependency)
		{
            _nugets.Add(new StubNuget(dependency, () => Repository.As<StubPackageRepository>().GetPackageByDependency(dependency)));
			return this;
		}

		public IRemoteNuget Find(Dependency query)
		{
            if (query.IsFloat() || query.Version.IsEmpty())
            {
                return _nugets.FirstOrDefault(x => x.Name == query.Name);
            }

			var version = SemanticVersion.Parse(query.Version);
			return _nugets.FirstOrDefault(x => x.Name == query.Name && x.Version.Equals(version));
		}

		public IRemoteNuget FindLatest(Dependency query)
		{
			return _nugets.Where(x => x.Name == query.Name)
			              .OrderByDescending(x => x.Version)
			              .FirstOrDefault();
		}

		public void ConfigureRepository(Action<StubPackageRepository> configure)
		{
			configure((StubPackageRepository)Repository);
		}

		public void UseRepository(StubPackageRepository repository)
		{
			Repository = repository;
		}

		public IPackageRepository Repository { get; private set; }
	}
}