using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;
using ripple.Model;

namespace ripple.Nuget
{
    public class FloatingFeed : NugetFeed, IFloatingFeed
    {
        public const string FindAllLatestCommand =
            "/Packages()?$filter=IsAbsoluteLatestVersion&$orderby=DownloadCount%20desc,Id&$skip=0&$top=1000";

		private readonly Lazy<XmlDocument> _feed;

        public FloatingFeed(string url, NugetStability stability) 
            : base(url, stability)
        {
            _feed = new Lazy<XmlDocument>(loadLatestFeed);
        }

        private XmlDocument loadLatestFeed()
        {
            var url = Url + FindAllLatestCommand;
            var client = new WebClient();
            var text = client.DownloadString(url);

            var document = new XmlDocument();
            document.LoadXml(text);

            return document;
        }

        public IEnumerable<IRemoteNuget> GetLatest()
        {
            var feed = new NugetXmlFeed(_feed.Value);
            return feed.ReadAll(this);
        }

        public IRemoteNuget LatestFor(Dependency dependency)
        {
            var floatedResult = GetLatest().SingleOrDefault(x => dependency.MatchesName(x.Name));
            if (floatedResult != null && dependency.Mode == UpdateMode.Fixed && floatedResult.IsUpdateFor(dependency))
            {
                return null;
            }

            return floatedResult;
        }
    }
}