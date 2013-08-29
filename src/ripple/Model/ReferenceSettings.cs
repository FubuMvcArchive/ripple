using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace ripple.Model
{
    public class ReferenceSettings
    {
        private readonly IList<IgnoreAssemblyReference> _ignores = new List<IgnoreAssemblyReference>();

        [XmlArray("IgnoreAssemblies"), XmlArrayItem("Ignore")]
        public IgnoreAssemblyReference[] IgnoredAssemblies
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