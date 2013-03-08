using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Logging;
using ripple.New.Commands;
using ripple.New.Nuget;

namespace ripple.New.Model
{
	[XmlType("ripple")]
	public class Solution : DescribesItself, LogTopic
	{
		private readonly IList<Project> _projects = new List<Project>();
		private readonly IList<Feed> _feeds = new List<Feed>();
		private readonly IList<Dependency> _configuredDependencies = new List<Dependency>();
		private readonly Lazy<IEnumerable<Dependency>> _missing;
		private readonly Lazy<IEnumerable<IRemoteNuget>> _updates;
		private readonly Lazy<DependencyCollection> _dependencies; 

		public Solution()
		{
			NugetSpecFolder = "packaging/nuget";
			SourceFolder = "src";
			BuildCommand = "rake";
			FastBuildCommand = "rake compile";

			AddFeed(Feed.Fubu);
			AddFeed(Feed.NuGetV2);
			AddFeed(Feed.NuGetV1);

			UseStorage(new RippleStorage());
			UseFeedService(new FeedService());

			_missing = new Lazy<IEnumerable<Dependency>>(() => Storage.MissingFiles(this));
			_updates = new Lazy<IEnumerable<IRemoteNuget>>(findUpdates);
			_dependencies = new Lazy<DependencyCollection>(combineDependencies);
		}

		public string Name { get; set; }
		public string Path { get; set; }
		public string NugetSpecFolder { get; set; }
		public string SourceFolder { get; set; }
		public string BuildCommand { get; set; }
		public string FastBuildCommand { get; set; }

		[XmlIgnore]
		public INugetStorage Storage { get; private set; }
		[XmlIgnore]
		public IFeedService FeedService { get; private set; }

		private DependencyCollection combineDependencies()
		{
			var dependencies = new DependencyCollection(_configuredDependencies);
			Projects.Each(p => dependencies.AddChild(p.Dependencies));
			return dependencies;
		}

		public string PackagesDirectory()
		{
			return System.IO.Path.Combine(SourceFolder, "packages").ToFullPath();
		}

		public void UseStorage(INugetStorage storage)
		{
			Storage = storage;
		}

		public void UseFeedService(IFeedService service)
		{
			FeedService = service;
		}

		[XmlIgnore]
		public Project[] Projects
		{
			get { return _projects.ToArray(); }
			set
			{
				_projects.Clear();
				_projects.AddRange(value);
			}
		}

		public Feed[] Feeds
		{
			get { return _feeds.ToArray(); }
			set
			{
				_feeds.Clear();
				_feeds.AddRange(value);
			}
		}

		public Dependency[] Nugets
		{
			get { return _configuredDependencies.ToArray(); }
			set
			{
				_configuredDependencies.Clear();
				_configuredDependencies.AddRange(value);
			}
		}

		[XmlIgnore]
		public DependencyCollection Dependencies
		{
			get { return _dependencies.Value; }
		}

		public void AddFeed(Feed feed)
		{
			_feeds.Fill(feed);
		}

		public void AddProject(Project project)
		{
			project.Solution = this;
			_projects.Fill(project);
		}

		public void AddDependency(Dependency dependency)
		{
			_configuredDependencies.Fill(dependency);
		}

		public Dependency FindDependency(string name)
		{
			return _configuredDependencies.SingleOrDefault(x => x.Name == name);
		}

		public void ClearFeeds()
		{
			_feeds.Clear();
		}

		public IEnumerable<Dependency> MissingNugets()
		{
			return _missing.Value;
		}

		public IRemoteNuget Restore(Dependency dependency)
		{
			return FeedService.NugetFor(this, dependency);
		}

		public Project FindProject(string name)
		{
			return _projects.SingleOrDefault(x => x.Name.EqualsIgnoreCase(name));
		}

		public void Describe(Description description)
		{
			description.Title = "Solution \"{0}\"".ToFormat(Name);
			description.ShortDescription = Path;

			var feedsList = description.AddList("Feeds", Feeds);
			feedsList.Label = "NuGet Feeds";

			var projectsList = description.AddList("Projects", Projects);
			projectsList.Label = "Projects";

			var local = LocalDependencies();
			if (local.Any())
			{
				var localList = description.AddList("Local", local.All());
				localList.Label = "Local";
			}

			var missing = MissingNugets();
			if (missing.Any())
			{
				var missingList = description.AddList("Missing", missing);
				missingList.Label = "Missing";
			}
		}

		public void AssertIsValid()
		{
			var exception = new RippleException(this);

			_projects
				.SelectMany(x => x.Dependencies)
				.GroupBy(x => x.Name)
				.Each(group =>
				{
					var version = group.First().Version;
					if (group.Any(d => d.Version != version))
					{
						exception.AddProblem("Validation", "Multiple dependencies found for " + group.Key);
					}
				});

			if (exception.HasProblems())
			{
				throw exception;
			}
		}

		public LocalDependencies LocalDependencies()
		{
			return Storage.Dependencies(this);
		}

		public IEnumerable<IRemoteNuget> Updates()
		{
			return _updates.Value;
		}

		private IEnumerable<IRemoteNuget> findUpdates()
		{
			return FeedService.UpdatesFor(this);
		}

		public static Solution For(SolutionInput input)
		{
			var builder = SolutionBuilder.Basic();

			// TODO -- Need to allow a specific solution
			return builder.Build();
		}
	}
}