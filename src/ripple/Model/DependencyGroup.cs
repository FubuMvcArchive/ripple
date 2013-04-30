using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using FubuCore;

namespace ripple.Model
{
    public class GroupedDependency
    {
        public GroupedDependency()
        {
        }

        public GroupedDependency(string name)
        {
            Name = name;
        }

        [XmlAttribute]
        public string Name { get; set; }
    }

    public class DependencyGroup
    {
        public DependencyGroup()
        {
            Dependencies = new List<GroupedDependency>();
        }

        [XmlElement("Dependency")]
        public List<GroupedDependency> Dependencies { get; set; }

        public bool Has(string name)
        {
            return Dependencies.Any(x => x.Name.EqualsIgnoreCase(name));
        }
    }
}