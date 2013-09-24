using System.Collections.Generic;

namespace ripple.Model.Conversion
{
    public class NuGetConverter
    {
        public NuGetConversionReport Convert(Solution solution)
        {
            var report = new NuGetConversionReport();
            var analyzer = new DependencyAnalyzer();

            solution.Projects.Each(project => analyzer.Analyze(project, report));
            analyzer.Fill(solution);

            return report;
        }
    }
}