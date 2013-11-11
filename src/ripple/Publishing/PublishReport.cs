using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Logging;
using NuGet;

namespace ripple.Publishing
{
    public enum ReportCategory
    {
        success,
        failure
    }

    public interface IPublishReportItem : DescribesItself
    {
        ReportCategory Category { get; }
    }

    public class PublishSuccessful : IPublishReportItem
    {
        private readonly IPackage _package;

        public PublishSuccessful(IPackage package)
        {
            _package = package;
        }

        public ReportCategory Category { get { return ReportCategory.success; } }


        public void Describe(Description description)
        {
            description.Title = _package.Id;
            description.ShortDescription = "Successfully published {0}".ToFormat(_package.Version);
        }
    }

    public class VersionAlreadyExists : IPublishReportItem
    {
        private readonly IPackage _package;

        public VersionAlreadyExists(IPackage package)
        {
            _package = package;
        }

        public ReportCategory Category { get { return ReportCategory.failure; } }


        public void Describe(Description description)
        {
            description.Title = _package.Id;
            description.ShortDescription = "{0} already exists on the server".ToFormat(_package.Version);
        }
    }

    public class PublishFailure : IPublishReportItem
    {
        private readonly IPackage _package;
        private readonly Exception _exception;

        public PublishFailure(IPackage package, Exception exception)
        {
            _package = package;
            _exception = exception;
        }

        public ReportCategory Category { get { return ReportCategory.failure; } }

        public void Describe(Description description)
        {
            description.Title = _package.Id;
            description.ShortDescription = _exception.Message;
            description.AddChild("Stacktrace", _exception.StackTrace);
        }
    }

    public class PublishReport : LogTopic, DescribesItself
    {
        private readonly IList<IPublishReportItem> _items = new List<IPublishReportItem>();

        public void Add(IPublishReportItem item)
        {
            _items.Add(item);
        }

        public bool IsSuccessful()
        {
            return !Failures.Any();
        }

        public IEnumerable<IPublishReportItem> Successful
        {
            get { return _items.Where(x => x.Category == ReportCategory.success); }
        }

        public IEnumerable<IPublishReportItem> Failures
        {
            get { return _items.Where(x => x.Category == ReportCategory.failure); }
        }

        public void Describe(Description description)
        {
            description.Title = "Publish Report";
            description.ShortDescription = "";

            description.AddList("Details", _items);
        }
    }
}