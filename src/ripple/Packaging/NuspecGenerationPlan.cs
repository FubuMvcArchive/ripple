using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Logging;
using NuGet;
using ripple.Model;

namespace ripple.Packaging
{
    public class NuspecGenerationPlan : LogTopic, DescribesItself
    {
        private readonly Solution _solution;
        private readonly SemanticVersion _version;
        private readonly IList<NuspecPlan> _plans = new List<NuspecPlan>();

        public NuspecGenerationPlan(Solution solution, SemanticVersion version)
        {
            _solution = solution;
            _version = version;
        }

        public IEnumerable<NuspecPlan> Children { get { return _plans; } } 

        public NuspecPlan Child(string packageId)
        {
            return _plans.SingleOrDefault(x => x.Spec.Matches(packageId));
        }

        public void Add(NuspecPlan plan)
        {
            _plans.Fill(plan);
        }

        public void Describe(Description description)
        {
            description.Title = "Create Packages for " + _solution.Name;
            description.ShortDescription = "Version " + _version;

            description.AddList("Nuspecs", _plans);
        }

        public NuspecGenerationReport Execute(bool updateDependencies)
        {
            var rootPackagingDir = _solution.NugetSpecFolder.ParentDirectory();
            var outputDir = rootPackagingDir.AppendPath(Guid.NewGuid().ToString()).ToFullPath();
            var files = new FileSystem();

            files.ForceClean(outputDir);
            files.CreateDirectory(outputDir);

            RippleLog.Info("Generating nuspec templates at: " + outputDir);

            var report = new NuspecGenerationReport(outputDir, updateDependencies);

            _plans.Each(plan => plan.Generate(report));

            return report;
        }
    }
}