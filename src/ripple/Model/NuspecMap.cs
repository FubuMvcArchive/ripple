using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using ripple.Nuget;
using ripple.Packaging;

namespace ripple.Model
{
    // nuspec 'FubuCore', publishedBy: 'FubuCore', dependsOn: 'FubuCore.Interfaces'
    // nuspec 'FubuCore.Interfaces', publishedBy: 'FubuCore'
    // nuspec 'FubuCore.Interfaces', publishedBy: 'FubuCore.Support'

    public class NuspecMap
    {
        public string PackageId { get; set; }
        public string PublishedBy { get; set; }
        public string DependsOn { get; set; }

        public ProjectNuspec ToSpec(Solution solution)
        {
            var project = solution.FindProject(PublishedBy);
            if (project == null)
            {
                throw new InvalidOperationException(PublishedBy + " is not a valid project name");
            }

            var target = solution.Specifications.SingleOrDefault(x => x.Matches(PackageId));
            if (target == null)
            {
                throw new InvalidOperationException("Solution is not configured to publish " + PackageId);
            }

            var spec = new ProjectNuspec(project, target);
            if (DependsOn.IsNotEmpty())
            {
                var dependencies = DependsOn.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
                dependencies.Each(x =>
                {
                    var dependency = solution.Specifications.SingleOrDefault(s => s.Matches(x));
                    if (dependency == null)
                    {
                        throw new InvalidOperationException("Solution is not configured to publish " + x);
                    }

                    spec.AddDependency(dependency);
                });
            }

            return spec;
        }
    }
}