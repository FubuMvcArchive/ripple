using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml;
using FubuCore;
using FubuCore.CommandLine;
using System.Linq;

namespace ripple.Extract
{
    // /guestAuth/app/nuget/v1/FeedService.svc/Packages()?$filter=IsAbsoluteLatestVersion&$orderby=DownloadCount%20desc,Id&$skip=0&$top=30

    public class ExtractInput
    {
        public ExtractInput()
        {
            FeedFlag = RippleConstants.FubuTeamCityFeed;
            MaxFlag = 200;
        }

        [Description("Directory to dump the nuget files")]
        public string Directory { get; set; }

        [Description("Nuget feed.  If not specified, will use the Fubu TeamCity")]
        public string FeedFlag { get; set; }
        

        [Description("Maximum number of nugets to download, capped at 200 by default")]
        public int MaxFlag { get; set; }
    }

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


    public class NugetFeed
    {
        public NugetFeed(string url)
        {
            Url = url;
            File = url.Split('/').Last();
            Name = File.Split('.').First();
        }

        public string File { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }

        public override string ToString()
        {
            return string.Format("Name: {0}, Url: {1}, File: {2}", Name, Url, File);
        }

        public void DownloadTo(string directory)
        {
            var file = directory.AppendPath(File);
            var client = new WebClient();

            Console.WriteLine("Downloading {0} to {1}", Url, file);
            client.DownloadFile(Url, file);
        }
    }
}