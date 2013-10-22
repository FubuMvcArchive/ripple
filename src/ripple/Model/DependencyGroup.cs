using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;

namespace ripple.Model
{
    public class DependencyGroup
    {
        private readonly IList<GroupedDependency> _dependencies = new List<GroupedDependency>(); 

        public string Name { get; set; }

        public string Dependencies
        {
            get { return GroupedDependencies.OrderBy(x => x.Name).Select(x => x.Name).Join(","); }
            set { GroupedDependencies = GroupedDependency.Parse(value); }
        }

        public IEnumerable<GroupedDependency> GroupedDependencies
        {
            get { return _dependencies; }
            set
            {
                _dependencies.Clear();
                _dependencies.AddRange(value);
            }
        }

        public void Add(string name)
        {
            Add(new GroupedDependency(name));
        }

        public void Add(GroupedDependency dependency)
        {
            _dependencies.Add(dependency);
        }

        public bool Has(string name)
        {
            return GroupedDependencies.Any(x => x.Name.EqualsIgnoreCase(name));
        }
    }

    public class GroupedDependency
    {
        public GroupedDependency()
        {
        }

        public GroupedDependency(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public static IEnumerable<GroupedDependency> Parse(string input)
        {
            return input
                .Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                .Select(name => new GroupedDependency(name));
        }
    }
}