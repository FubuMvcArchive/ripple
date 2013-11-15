using System.Collections.Generic;
using FubuCore;
using FubuCore.Descriptions;
using NuGet;
using ripple.Nuget;

namespace ripple.Packaging
{
    public class NuspecPlan : DescribesItself
    {
        private readonly NuspecTemplate _template;
        private readonly SemanticVersion _version;
        private readonly IList<NuspecDependencyToken> _dependencies = new List<NuspecDependencyToken>();

        public NuspecPlan(NuspecTemplate template, SemanticVersion version)
        {
            _template = template;
            _version = version;
        }

        public NugetSpec Spec { get { return _template.Spec; } }
        public IEnumerable<NuspecDependencyToken> Dependencies { get { return _dependencies; } }

        public void AddDependency(NuspecDependencyToken dependency)
        {
            _dependencies.Fill(dependency);
        }

        public void AddDependencies(IEnumerable<NuspecDependencyToken> dependencies)
        {
            _dependencies.Fill(dependencies);
        }

        protected bool Equals(NuspecPlan other)
        {
            return _template.Equals(other._template);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NuspecPlan) obj);
        }

        public override int GetHashCode()
        {
            return _template.GetHashCode();
        }

        public void Describe(Description description)
        {
            description.Title = Spec.Name;
            description.ShortDescription = "Version " + _version;

            description.AddList("Dependencies", _dependencies);
        }

        public void Generate(NuspecGenerationReport report)
        {
            var files = new FileSystem();
            var targetFile = report.FindNuspecFile(_template);
            
            files.Copy(_template.Spec.Filename, targetFile);

            if (report.UpdateDependencies)
            {
                generateNuspec(targetFile);
            }
        }

        private void generateNuspec(string targetFile)
        {
            var nuspec = new NuspecDocument(targetFile);
            _dependencies.Each(token =>
            {
                var dependency = token.Create();
                nuspec.AddDependency(dependency);
            });

            nuspec.SaveChanges();
        }
    }
}