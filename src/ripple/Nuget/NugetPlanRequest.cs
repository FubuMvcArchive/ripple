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
        public string Version { get; set; }
        public OperationType Operation { get; set; }
        public UpdateMode Mode { get; set; }

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