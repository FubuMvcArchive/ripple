using System;
using System.Linq;
using System.Net;
using FubuCore;

namespace ripple.Extract
{
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