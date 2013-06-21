using System.Collections.Generic;
using System.Linq;
using FubuCore;
using ripple.Model;

namespace ripple.Commands
{
	// TODO -- Remove when it is possible to have IEnumerable<string> flag
	public static class StringFeeds
	{
		public static IEnumerable<string> ParseFeeds(this string urlString)
		{
			return urlString.IsNotEmpty()
				       ? urlString.ToDelimitedArray('#')
				       : Enumerable.Empty<string>();
		}

		public static IEnumerable<Feed> GetFeeds(this string feedsFlag)
		{
			if (feedsFlag.IsEmpty())
			{
				return new Feed[0];
			}

			return feedsFlag
				.ParseFeeds()
				.Select(Feed.FindOrCreate);
		}
	}
}