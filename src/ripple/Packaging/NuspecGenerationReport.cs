using System.Collections.Generic;
using FubuCore;
using FubuCore.Util;

namespace ripple.Packaging
{
    public class NuspecGenerationReport
    {
        private readonly string _outputDir;
        private readonly bool _updateDependencies;
        private readonly Cache<NuspecTemplate, string> _templates = new Cache<NuspecTemplate, string>(); 

        public NuspecGenerationReport(string outputDir, bool updateDependencies)
        {
            _outputDir = outputDir;
            _updateDependencies = updateDependencies;

            _templates.OnMissing = template => _outputDir.AppendPath(template.Id + ".nuspec");
        }

        public IEnumerable<string> NuspecFiles { get { return _templates; } } 

        public string OutputDir
        {
            get { return _outputDir; }
        }

        public bool UpdateDependencies
        {
            get { return _updateDependencies; }
        }

        public string FindNuspecFile(NuspecTemplate template)
        {
            return _templates[template];
        }
    }
}