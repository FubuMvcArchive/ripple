using System.Collections.Generic;
using System.Linq;
using ripple.Model;

namespace ripple.Nuget
{
    public class SpecGroup
    {
        public SpecGroup(NugetSpec spec, IEnumerable<Project> projects)
        {
            Spec = spec;
            Projects = projects;
        }

        public NugetSpec Spec { get; private set; }
        public IEnumerable<Project> Projects { get; private set; }

        public IEnumerable<Dependency> DetermineDependencies()
        {
            return Projects
                .SelectMany(project => project.Dependencies
                .Where(x => !Spec.Dependencies.Any(y => y.MatchesName(x.Name)))
                .Select(x => project.Solution.Dependencies.Find(x.Name)));
        }
    }

    public class ProjectNuspec
    {
        private readonly Project _project;
        private readonly NugetSpec _spec;

        public ProjectNuspec(Project project, NugetSpec spec)
        {
            _project = project;
            _spec = spec;
        }

        public Project Project
        {
            get { return _project; }
        }

        public NugetSpec Spec
        {
            get { return _spec; }
        }

        protected bool Equals(ProjectNuspec other)
        {
            return _project.Equals(other._project) && _spec.Equals(other._spec);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProjectNuspec) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_project.GetHashCode()*397) ^ _spec.GetHashCode();
            }
        }
    }
}