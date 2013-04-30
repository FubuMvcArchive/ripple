using FubuCore;
using ripple.Model;

namespace ripple.Nuget
{
    public enum OperationType
    {
        Install,
        Update
    }

    public class NugetPlanRequest
    {
        public Solution Solution { get; set; }
        public Dependency Dependency { get; set; }
        public string Project { get; set; }
        public OperationType Operation { get; set; }
        public NugetPlanRequest Parent { get; set; }
        public bool Batched { get; set; }

        /// <summary>
        /// Fixed versions are 'fixed'. Use this option to force updates of existing dependencies.
        /// </summary>
        public bool ForceUpdates { get; set; }

        public bool IsTransitive()
        {
            return Parent != null;
        }

        public bool InstallToProject()
        {
            return Project.IsNotEmpty();
        }

        public bool UpdatesCurrentDependency()
        {
            if (!Solution.Dependencies.Has(Dependency.Name)) return false;

            var configured = Solution.Dependencies.Find(Dependency.Name);

            if (!Solution.LocalDependencies().Has(Dependency))
            {
                if (configured.IsFloat()) return true;

                return configured.SemanticVersion() < Dependency.SemanticVersion();
            }

            var local = Solution.LocalDependencies().Get(Dependency);
            return local.Version < Dependency.SemanticVersion();
        }

        public NugetPlanRequest CopyFor(Dependency dependency)
        {
            var request = (NugetPlanRequest) MemberwiseClone();
            request.Dependency = dependency;
            request.Parent = this;

            return request;
        }

        public Dependency Origin()
        {
            var origin = Dependency;
            var parent = Parent;
            while (parent != null)
            {
                origin = parent.Dependency;
                parent = parent.Parent;
            }

            return origin;
        }

        public bool ShouldForce(Dependency dependency)
        {
            if (ForceUpdates) return true;

            return !Batched && Origin().MatchesName(dependency);
        }

        public bool ShouldUpdate(Dependency configured)
        {
            return (IsTransitive() || Operation == OperationType.Update) && (ShouldForce(configured) || configured.IsFloat());
        }

        protected bool Equals(NugetPlanRequest other)
        {
            return Equals(Solution, other.Solution) && Equals(Dependency, other.Dependency) && Operation == other.Operation;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NugetPlanRequest)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Solution != null ? Solution.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Dependency != null ? Dependency.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)Operation;
                return hashCode;
            }
        }
    }
}