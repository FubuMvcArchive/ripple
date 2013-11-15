using System.Collections.Generic;
using ripple.Model;
using ripple.Nuget;

namespace ripple.Packaging
{
    public class ProjectNuspec
    {
        private readonly Project _project;
        private readonly NugetSpec _publishes;
        private readonly IList<NugetSpec> _dependencies = new List<NugetSpec>();

        public ProjectNuspec(Project project, NugetSpec publishes)
        {
            _project = project;
            _publishes = publishes;
        }

        public Project Project
        {
            get { return _project; }
        }

        public NugetSpec Publishes
        {
            get { return _publishes; }
        }

        public IEnumerable<NugetSpec> Dependencies { get { return _dependencies; } } 

        public void AddDependency(NugetSpec spec)
        {
            _dependencies.Add(spec);
        }

        protected bool Equals(ProjectNuspec other)
        {
            return _project.Equals(other._project) && _publishes.Equals(other._publishes);
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
                return (_project.GetHashCode()*397) ^ _publishes.GetHashCode();
            }
        }

        /// <summary>
        /// Mostly for testing
        /// </summary>
        /// <param name="project"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ProjectNuspec For(Project project)
        {
            return new ProjectNuspec(project, new NugetSpec(project.Name, project.Name + ".nuspec"));
        }
    }
}