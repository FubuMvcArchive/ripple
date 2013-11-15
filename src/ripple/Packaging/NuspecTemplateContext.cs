using NuGet;
using ripple.Model;

namespace ripple.Packaging
{
    public class NuspecTemplateContext
    {
        public NuspecTemplateContext(NuspecTemplate current, NuspecTemplateCollection templates, Solution solution)
            : this(current, templates, solution, null)
        {
        }

        public NuspecTemplateContext(NuspecTemplate current, NuspecTemplateCollection templates, Solution solution, SemanticVersion version)
        {
            Templates = templates;
            Version = version;
            Solution = solution;
            Current = current;
        }

        public NuspecTemplate Current { get; private set; }
        public Solution Solution { get; private set; }
        public SemanticVersion Version { get; private set; }
        public NuspecTemplateCollection Templates { get; private set; }

        protected bool Equals(NuspecTemplateContext other)
        {
            return Current.Equals(other.Current);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NuspecTemplateContext) obj);
        }

        public override int GetHashCode()
        {
            return Current.GetHashCode();
        }
    }
}