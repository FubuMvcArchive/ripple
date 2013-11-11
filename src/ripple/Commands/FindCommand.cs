using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.CommandLine;
using NuGet;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Commands
{
    public class FindNugetsInput : RippleInput
    {
        public string Nuget { get; set; }
    }

    [CommandDescription("Queries feeds for nugets", Name = "find")]
    public class FindNugetsCommand : FubuCommand<FindNugetsInput>
    {
        public override bool Execute(FindNugetsInput input)
        {
            var solution = Solution.For(input);
            var feeds = FeedRegistry.FeedsFor(solution);
            var results = feeds.SelectMany(feed =>
            {
                return feed
                    .FindAllLatestByName(input.Nuget)
                    .Select(nuget => new SearchResult
                    {
                        Nuget = nuget,
                        Provenance = feed.Repository
                    });
            });

            results
                .OrderBy(x => x.Nuget.Name)
                .ThenBy(x => x.Nuget.Version)
                .Each(result =>
                {
                    RippleLog.Info("{0}, {1} ({2})".ToFormat(result.Nuget.Name, result.Nuget.Version, result.Provenance.Source));
                });


            return true;
        }

        public class SearchResult
        {
            public IRemoteNuget Nuget { get; set; }
            public IPackageRepository Provenance { get; set; }
        }
    }
}