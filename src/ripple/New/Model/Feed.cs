using System.Xml.Serialization;
using FubuCore;
using FubuCore.Descriptions;
using ripple.New.Nuget;

namespace ripple.New.Model
{
	public class Feed : DescribesItself
	{
		public static readonly Feed NuGetV1 = new Feed("http://packages.nuget.org/v1/FeedService.svc/");
		public static readonly Feed NuGetV2 = new Feed("http://nuget.org/api/v2");
		public static readonly Feed Fubu = new Feed("http://build.fubu-project.org/guestAuth/app/nuget/v1/FeedService.svc", UpdateMode.Float);

		public Feed()
		{
		}

		public Feed(string url)
			: this(url, UpdateMode.Fixed)
		{
		}

		public Feed(string url, UpdateMode mode)
		{
			Url = url;
			Mode = mode;
		}

		[XmlAttribute]
		public string Url { get; set; }
		[XmlAttribute]
		public UpdateMode Mode { get; set; }

		public INugetFeed GetNugetFeed()
		{
			return FeedRegistry.FeedFor(this);
		}

		// This is stupid
		public IRemoteNuget Find(Dependency query)
		{
			var feed = GetNugetFeed();
			if (Mode == UpdateMode.Float || query.IsFloat())
			{
				return feed.FindLatest(query);
			}

			return feed.Find(query);
		}

		protected bool Equals(Feed other)
		{
			return string.Equals(Url, other.Url);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((Feed) obj);
		}

		public override int GetHashCode()
		{
			return Url.GetHashCode();
		}

		public void Describe(Description description)
		{
			description.ShortDescription = "{0} ({1})".ToFormat(Url, Mode);
		}
	}
}