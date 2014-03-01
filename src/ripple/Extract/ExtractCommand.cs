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
			"/Packages()?$filter=IsAbsoluteLatestVersion&$orderby=DownloadCount%20desc,Id&$skip={0}";

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
		    var i = 0;
		    var pageSize = 100;
		    var lastFetchCount = pageSize;

		    while (lastFetchCount == pageSize)
		    {
		        var skip = i++*pageSize;
                var url = input.FeedFlag.TrimEnd('/') + Command.ToFormat(skip);

                var document = XDocument.Load(url);

		        var entries =
		            document.Root.Elements()
		                .Where(x => x.Name.LocalName == "entry")
		                .Select(e => new NugetEntry(e)).ToArray();

		        foreach (var nugetEntry in entries.Where(x => !x.Name.EndsWith(".Docs")))
		        {
		            yield return nugetEntry;
		        }

		        lastFetchCount = entries.Length;
		    }
		}
	}
}