using System;
using System.Collections.Generic;
using System.Linq;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Packaging
{
    public class NuspecTemplate
    {
        private readonly IList<ProjectNuspec> _projectNuspecs = new List<ProjectNuspec>();

        public NuspecTemplate(NugetSpec spec, IEnumerable<ProjectNuspec> projects)
        {
            Spec = spec;
            _projectNuspecs.AddRange(projects);

            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
        public NugetSpec Spec { get; private set; }
        public IEnumerable<Project> Projects { get { return _projectNuspecs.Select(x => x.Project); } }

        public IEnumerable<NugetSpec> NugetSpecDependencies()
        {
            return _projectNuspecs.SelectMany(x => x.Dependencies);
        }

        public IEnumerable<Dependency> DetermineDependencies()
        {
            return Projects
                .SelectMany(project => project
                    .Dependencies
                    .Select(x => project.Solution.Dependencies.Find(x.Name)))
                .Distinct();
        }

        protected bool Equals(NuspecTemplate other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NuspecTemplate) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}