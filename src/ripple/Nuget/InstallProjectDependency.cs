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
    }
}