using System;
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

        public bool InstallToProject()
        {
            return Project.IsNotEmpty();
        }

        public bool UpdatesCurrentDependency()
        {
            // Going to have to compare against the latest from the feed here
            if (Dependency.IsFloat()) return true;

            if (!Solution.Dependencies.Has(Dependency.Name)) return false;

            var configured = Solution.Dependencies.Find(Dependency.Name);
            var local = Solution.LocalDependencies().Get(Dependency);

            if (local == null)
            {
                if (configured.IsFloat()) return true;

                return configured.SemanticVersion() < Dependency.SemanticVersion();
            }

            return local.Version < Dependency.SemanticVersion();
        }

        /// <summary>
        /// Fixed versions are 'fixed'. Use this option to force updates of existing dependencies.
        /// </summary>
        public bool ForceUpdates { get; set; }

        public NugetPlanRequest CopyFor(Dependency dependency)
        {
            var request = (NugetPlanRequest) MemberwiseClone();
            request.Dependency = dependency;

            return request;
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