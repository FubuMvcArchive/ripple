using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using FubuCore;
using FubuCore.Descriptions;

namespace ripple.New
{
	[XmlType("ripple")]
	public class Solution : DescribesItself
	{
		private readonly IList<Project> _projects = new List<Project>();
		private readonly IList<Feed> _feeds = new List<Feed>();

		public Solution()
		{
			NugetSpecFolder = "packaging/nuget";
			SourceFolder = "src";
			BuildCommand = "rake";
			FastBuildCommand = "rake compile";
		}

		public string Name { get; set; }
		public string Path { get; set; }
		public string NugetSpecFolder { get; set; }
		public string SourceFolder { get; set; }
		public string BuildCommand { get; set; }
		public string FastBuildCommand { get; set; }

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
			project.Solution = this;
			_projects.Fill(project);
		}

		public void ClearFeeds()
		{
			_feeds.Clear();
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
		}
	}
}