using System.ComponentModel;
using System.Diagnostics;
using System.IO;

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
}