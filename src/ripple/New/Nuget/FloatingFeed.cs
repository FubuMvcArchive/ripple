using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace ripple.New.Nuget
{
    public class FloatingFeed : NugetFeed, IFloatingFeed
    {
        public const string FindAllLatestCommand =
            "/Packages()?$filter=IsAbsoluteLatestVersion&$orderby=DownloadCount%20desc,Id&$skip=0&$top=1000";

		private readonly Lazy<XmlDocument> _feed;

        public FloatingFeed(string url) : base(url)
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
            return feed.ReadAll();
        }
    }
}