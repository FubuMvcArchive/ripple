using System.Collections.Generic;
using System.IO;
using FubuCore;

namespace ripple.Model.Conversion
{
    public class NuGetConverter
    {
        public NuGetConversionReport Convert(Solution solution)
        {
            var report = new NuGetConversionReport();
            var analyzer = new DependencyAnalyzer();

            solution.Projects.Each(project =>
            {
                analyzer.Analyze(project, report);
                project.Proj.ConvertToRippleDependenciesConfig();

                var directory = project.FilePath.ParentDirectory();
                var packagesConfig = directory.AppendPath("packages.config").ToFullPath();
                
                if (File.Exists(packagesConfig))
                {
                    File.Delete(packagesConfig);
                }
            });

            analyzer.Fill(solution);

            return report;
        }
    }
}