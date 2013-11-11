using System.Collections.Generic;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Publishing
{
    public interface IPublishingService
    {
        IEnumerable<NugetSpec> SpecificationsFor(Solution solution);
        string CreatePackage(PackageParams ctx);

        IPublishReportItem PublishPackage(string serverUrl, string file, string apiKey);
    }
}
