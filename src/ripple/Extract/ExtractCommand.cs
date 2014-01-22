using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using FubuCore;
using FubuCore.CommandLine;

namespace ripple.Extract
{
	[CommandDescription("Extracts all the latest nugets from a remote feed ")]
	public class ExtractCommand : FubuCommand<ExtractInput>
	{
		public const string Command =
			"/Packages()?$filter=IsAbsoluteLatestVersion&$orderby=DownloadCount%20desc,Id&$skip=SKIP&$top=";

		public override bool Execute(ExtractInput input)
		{
			var system = new FileSystem();
			system.DeleteDirectory(input.Directory);
			system.CreateDirectory(input.Directory);

			var feeds = loadNugetEntries(input).ToList();
		    var count = 0;

		    var failed = new List<string>();

			feeds.Each(feed =>
			{
                Console.WriteLine("Downloading {0}/{1} - {2}", ++count, feeds.Count, feed.Name);

			    try
			    {
                    feed.DownloadTo(input.Directory);
			    }
			    catch (Exception e)
			    {
			        Console.WriteLine("Error!");
                    Console.WriteLine(e.Message);
                    failed.Add(feed.Name + " " + feed.Version);
			    }

				
			});

            if (failed.Any())
            {
                ConsoleWriter.Write(ConsoleColor.Red, "Nuget's failed to download:");
                failed.Each(x => Console.WriteLine(x));
            }

			return true;
		}

		private static readonly XNamespace atomNS = "http://www.w3.org/2005/Atom";

		private static IEnumerable<NugetEntry> loadNugetEntries(ExtractInput input)
		{
		    int count = 100;
		    int attempt = 0;
		    while (count == 100)
		    {
                var url = input.FeedFlag.TrimEnd('/') + Command.Replace("SKIP", (attempt * 100).ToString()) + input.MaxFlag;

                var document = XDocument.Load(url);
                var entries = document.Root.Elements().Where(x => x.Name.LocalName == "entry");
		        count = entries.Count();

                foreach (var entry in entries.Select(e => new NugetEntry(e)).Where(x => !x.Name.EndsWith(".Docs")))
                {
                    yield return entry;
                }

		        attempt++;
		    }


		}
	}
}