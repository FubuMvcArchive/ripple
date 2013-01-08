using System;
using System.Net;
using System.Xml;
using FubuCore;
using FubuCore.CommandLine;

namespace ripple.Extract
{
    [CommandDescription("Extracts all the latest nugets from a remote feed ")]
    public class ExtractCommand : FubuCommand<ExtractInput>
    {
        public const string Command =
            "/Packages()?$filter=IsAbsoluteLatestVersion&$orderby=DownloadCount%20desc,Id&$skip=0&$top=";

        public override bool Execute(ExtractInput input)
        {
            var system = new FileSystem();
            system.DeleteDirectory(input.Directory);
            system.CreateDirectory(input.Directory);

            var document = loadListOfNugets(input);
            foreach (XmlElement element in document.DocumentElement.ChildNodes)
            {
                if (element.Name == "entry")
                {
                    var feed = feedFor(element);

                    feed.DownloadTo(input.Directory);

                    Console.WriteLine(feed);
                }
            }



            return true;
        }

        public NugetFeed feedFor(XmlNode node)
        {
            foreach ( XmlElement element in node.ChildNodes)
            {
                if (element.Name == "content")
                {
                    var url = element.GetAttribute("src");
                    return new NugetFeed(url);
                }
            }

            throw new NotImplementedException("I can't find anything here --> \n\n" + node.OuterXml);
        }

        private static XmlDocument loadListOfNugets(ExtractInput input)
        {
            var url = input.FeedFlag.TrimEnd('/') + Command + input.MaxFlag;
            var client = new WebClient();
            var text = client.DownloadString(url);

            var document = new XmlDocument();
            document.LoadXml(text);

            return document;
        }
    }
}