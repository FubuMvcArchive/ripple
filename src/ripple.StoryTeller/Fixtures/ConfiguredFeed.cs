using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Util;
using NuGet;
using ripple.New.Model;
using ripple.New.Nuget;

namespace ripple.StoryTeller.Fixtures
{
	public class ConfiguredFeedCollection
	{
		public const string Fubu = "Fubu";
		public const string Nuget = "Nuget";

		private readonly Cache<string, Feed> _feeds;

		public ConfiguredFeedCollection()
		{
			_feeds = new Cache<string, Feed>(name => new Feed(name));

			_feeds.Fill(Fubu, Feed.Fubu);
			_feeds.Fill(Nuget, Feed.NuGetV2);
		}

		public Feed FeedFor(string name)
		{
			return _feeds[name];
		}

		public void ModeFor(string name, UpdateMode mode)
		{
			FeedFor(name).Mode = mode;
		}
	}

	public class ConfiguredFeedProvider : IFeedProvider
	{
		private readonly Cache<Feed, INugetFeed> _configuredFeeds;
		private readonly ConfiguredFeedCollection _collection;

		public ConfiguredFeedProvider(ConfiguredFeedCollection collection)
		{
			_collection = collection;
			_configuredFeeds = new Cache<Feed, INugetFeed>(feed => feed.Mode == UpdateMode.Fixed ? new ConfiguredFeed() : new ConfiguredFloatingFeed());
		}

		public INugetFeed For(Feed feed)
		{
			return _configuredFeeds[feed];
		}

		public ConfiguredFeed For(string name)
		{
			return For(_collection.FeedFor(name)).As<ConfiguredFeed>();
		}
	}

	public class ConfiguredFeed : INugetFeed
	{
		protected readonly IList<IRemoteNuget> Nugets = new List<IRemoteNuget>();

		public void Add(string name, string version)
		{
			Add(new Dependency(name, version));
		}

		public void Add(Dependency dependency)
		{
			Nugets.Add(new ConfiguredNuget(dependency));
		}

		public IRemoteNuget Find(Dependency query)
		{
			var version = SemanticVersion.Parse(query.Version);
			return Nugets.FirstOrDefault(x => x.Name == query.Name && x.Version.Equals(version));
		}

		public IRemoteNuget FindLatest(Dependency query)
		{
			return Nugets.Where(x => x.Name == query.Name)
			              .OrderByDescending(x => x.Version)
			              .FirstOrDefault();
		}
	}

	public class ConfiguredFloatingFeed : ConfiguredFeed, IFloatingFeed
	{
		public IEnumerable<IRemoteNuget> GetLatest()
		{
			var names = Nugets.Select(x => x.Name).Distinct().ToArray();
			var nugets = new List<IRemoteNuget>();

			names.Each(x =>
			{
				var latest = Nugets.Where(nuget => nuget.Name == x).OrderByDescending(nuget => nuget.Version).First();
				nugets.Add(latest);
			});

			return nugets;
		}
	}

	public class ConfiguredNuget : IRemoteNuget
	{
		public ConfiguredNuget(Dependency dependency)
			: this(dependency.Name, dependency.Version)
		{
		}

		public ConfiguredNuget(string name, string version)
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