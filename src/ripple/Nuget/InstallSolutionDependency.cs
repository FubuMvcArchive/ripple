using FubuCore;
using ripple.Model;

namespace ripple.Nuget
{
    public class InstallSolutionDependency : INugetStep
    {
        private readonly Dependency _dependency;

        public InstallSolutionDependency(Dependency dependency)
        {
            _dependency = dependency;
        }

        public void Execute(INugetStepRunner runner)
        {
            runner.AddSolutionDependency(_dependency);
        }

        protected bool Equals(InstallSolutionDependency other)
        {
            return Equals(_dependency, other._dependency) && _dependency.Mode == other._dependency.Mode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((InstallSolutionDependency)obj);
        }

        public override int GetHashCode()
        {
            return (_dependency != null ? _dependency.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return "Install {0} to solution".ToFormat(_dependency);
        }
    }
}