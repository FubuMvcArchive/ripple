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
	public class Repository : DescribesItself, LogTopic
	{
		private readonly IList<Project> _projects = new List<Project>();
		private readonly IList<Feed> _feeds = new List<Feed>();
		private readonly Lazy<IEnumerable<NugetQuery>> _missing;

		public Repository()
		{
			NugetSpecFolder = "packaging/nuget";
			SourceFolder = "src";
			BuildCommand = "rake";
			FastBuildCommand = "rake compile";

			_missing = new Lazy<IEnumerable<NugetQuery>>(() => Storage.MissingFiles(this));
		}

		public string Name { get; set; }
		public string Path { get; set; }
		public string NugetSpecFolder { get; set; }
		public string SourceFolder { get; set; }
		public string BuildCommand { get; set; }
		public string FastBuildCommand { get; set; }

		[XmlIgnore]
		public INugetStorage Storage { get; private set; }

		public string PackagesDirectory()
		{
			return System.IO.Path.Combine(SourceFolder, "packages").ToFullPath();
		}

		public void UseStorage(INugetStorage storage)
		{
			Storage = storage;
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

		public void AddFeed(Feed feed)
		{
			_feeds.Fill(feed);
		}

		public void AddProject(Project project)
		{
			project.Repository = this;
			_projects.Fill(project);
		}

		public void ClearFeeds()
		{
			_feeds.Clear();
		}

		public IEnumerable<NugetDependency> AllDependencies()
		{
			return _projects.SelectMany(x => x.Dependencies);
		}

		public IEnumerable<NugetQuery> MissingNugets()
		{
			return _missing.Value;
		}

		public Project FindProject(string name)
		{
			return _projects.SingleOrDefault(x => x.Name.EqualsIgnoreCase(name));
		}

		public void Describe(Description description)
		{
			description.Title = "Repository \"{0}\"".ToFormat(Name);
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

		public static Repository For(SolutionInput input)
		{
			var builder = RepositoryBuilder.Basic();

			// TODO -- Need to allow a specific solution
			return builder.Build();
		}
	}
}