using System;
using System.Collections.Generic;
using System.Net;

namespace ripple.New.Nuget
{
    public class UrlNugetDownloader : INugetDownloader
    {
        private readonly string _url;

        public UrlNugetDownloader(string url)
        {
            _url = url;
        }

        public string Url
        {
            get { return _url; }
        }

        public INugetFile DownloadTo(string filename)
        {
            var client = new WebClient();

            Console.WriteLine("Downloading {0} to {1}", Url, filename);
            client.DownloadFile(Url, filename);

            return new NugetFile(filename);
        }

        private sealed class UrlEqualityComparer : IEqualityComparer<UrlNugetDownloader>
        {
            public bool Equals(UrlNugetDownloader x, UrlNugetDownloader y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x._url, y._url);
            }

            public int GetHashCode(UrlNugetDownloader obj)
            {
                return (obj._url != null ? obj._url.GetHashCode() : 0);
            }
        }

        private static readonly IEqualityComparer<UrlNugetDownloader> UrlComparerInstance = new UrlEqualityComparer();

        public static IEqualityComparer<UrlNugetDownloader> UrlComparer
        {
            get { return UrlComparerInstance; }
        }
    }
}