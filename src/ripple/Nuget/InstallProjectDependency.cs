using System.Collections.Generic;
using System.Linq;
using FubuCore;
using ripple.Model;

namespace ripple.Nuget
{
    public class InstallProjectDependency : INugetStep
    {
        private readonly string _project;
        private readonly Dependency _dependency;

        public InstallProjectDependency(string project, Dependency dependency)
        {
            _project = project;
            _dependency = dependency;
        }

        public void Execute(INugetStepRunner runner)
        {
            runner.AddProjectDependency(_project, _dependency);
        }

        public Dependency Dependency
        {
            get { return _dependency; }
        }

        public void AddSelf(IList<INugetStep> steps)
        {
            var other = steps.OfType<InstallProjectDependency>().Where(x => x.Dependency.Name.EqualsIgnoreCase(Dependency.Name) && x._project.Equals(_project)).SingleOrDefault();

            if (other == null)
            {
                steps.Add(this);
                return;
            }

            if (Equals(other.Dependency, Dependency))
            {
                return;
            }

            if (Dependency.Mode != UpdateMode.Fixed) return;
            if (other.Dependency.Mode == UpdateMode.Float)
            {
                steps.Remove(other);
                steps.Add(this);
            }
            
        }

        protected bool Equals(InstallProjectDependency other)
        {
            return string.Equals(_project, other._project) && _dependency.Equals(other._dependency);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((InstallProjectDependency)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_project.GetHashCode() * 397) ^ _dependency.GetHashCode();
            }
        }

        public override string ToString()
        {
            return "Install {0} to {1}".ToFormat(_dependency.Name, _project);
        }
    }
}