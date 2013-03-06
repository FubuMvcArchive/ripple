using System.Collections.Generic;
using System.Linq;
using NuGet;
using ripple.New.Model;
using ripple.New.Nuget;

namespace ripple.Testing.New.Model
{
	public class StubFeedProvider : IFeedProvider
	{
		private readonly StubFeed _feed;

		public StubFeedProvider(StubFeed feed)
		{
			_feed = feed;
		}

		public INugetFeed For(Feed feed)
		{
			return _feed;
		}
	}

	public class StubFeed : INugetFeed
	{
		private readonly IList<IRemoteNuget> _nugets = new List<IRemoteNuget>();

		public void Add(string name, string version)
		{
			Add(new Dependency(name, version));
		}

		public void Add(Dependency dependency)
		{
			_nugets.Add(new StubNuget(dependency));
		}

		public IRemoteNuget Find(Dependency query)
		{
			var version = SemanticVersion.Parse(query.Version);
			return _nugets.FirstOrDefault(x => x.Name == query.Name && x.Version.Equals(version));
		}

		public IRemoteNuget FindLatest(Dependency query)
		{
			return _nugets.Where(x => x.Name == query.Name)
			              .OrderByDescending(x => x.Version)
			              .FirstOrDefault();
		}
	}

	public class StubNuget : IRemoteNuget
	{
		public StubNuget(Dependency dependency)
			: this(dependency.Name, dependency.Version)
		{
		}

		public StubNuget(string name, string version)
		{
			Name = name;
			Version = SemanticVersion.Parse(version);
		}

		public string Name { get; private set; }
		public SemanticVersion Version { get; private set; }

		public INugetFile DownloadTo(string directory)
		{
			throw new System.NotImplementedException();
		}

		public string Filename { get; private set; }
	}
}