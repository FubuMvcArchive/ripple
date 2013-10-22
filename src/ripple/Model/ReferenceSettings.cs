using System.Collections.Generic;
using System.Linq;

namespace ripple.Model
{
    public class ReferenceSettings
    {
        private readonly IList<IgnoreAssemblyReference> _ignores = new List<IgnoreAssemblyReference>();
        
        public IEnumerable<IgnoreAssemblyReference> IgnoredAssemblies
        {
            get { return _ignores.ToArray(); }
            set
            {
                _ignores.Clear();
                _ignores.AddRange(value);
            }
        }

        public void Ignore(string name, string assembly)
        {
            _ignores.Add(new IgnoreAssemblyReference { Package = name, Assembly = assembly});
        }

        public bool ShouldAddReference(Dependency dependency, string assemblyName)
        {
            return !_ignores.Any(x => x.Matches(dependency, assemblyName));
        }
    }
}